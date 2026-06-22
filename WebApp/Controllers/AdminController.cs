using ClosedXML.Excel;
using Dependency;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Business_Model;
using Service.Service;
using System.Text.RegularExpressions;
using WebApp.View_Model;
using System.Text.RegularExpressions;

namespace WebApp.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController(
    MangaService mangaService,
    UserService userService,
    ChapterService chapterService,
    ChapterUrlService chapterUrlService,
    CoverUrlService coverUrlService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var manga = await mangaService.GetAllManga();

        return View(manga.Select(MapToMangaViewModel).ToList());
    }

    [HttpPost]
    public async Task<IActionResult> ClickManga(int id)
    {
        if (id == 0)
        {
            return NotFound();
        }
        var manga = await mangaService.GetMangaById(id);
        if (manga is null)
        {
            return NotFound();
        }
        var viewModel = new List<MangaItem>
        {
            MapToMangaViewModel(manga)
        };

        return View("Views/Admin/Manga.cshtml", viewModel);
    }
    public async Task<IActionResult> EditChapter()
    {
        return View(EditChapter);
    }

    public async Task<IActionResult> Manga(string? q = null)
    {
        var manga = await mangaService.GetAllManga();
        var viewModel = manga.Select(MapToMangaViewModel).ToList();
        viewModel = FilterManga(viewModel, q).ToList();
        ViewData["SearchQuery"] = q ?? string.Empty;

        return View(viewModel);
    }

    public async Task<IActionResult> Users()
    {
        var users = await userService.GetAllUsers();

        return View(users.Select(MapToUserViewModel).ToList());
    }

    [HttpGet]
    public IActionResult AddManga()
    {
        return View(new MangaItem { Status = "Ongoing", Rating = 4.5m, Chapters = 0 });
    }

    [HttpGet]
    public async Task<IActionResult> ChapterLists(int id)
    {
        var manga = await mangaService.GetMangaById(id);
        if (manga is null)
        {
            return NotFound();
        }
        var chapters = await chapterService.GetChaptersByMangaId(id);
        var viewModel = MapToMangaViewModel(manga);
        viewModel.ChapterLists = chapters.Select(MapToChapterViewModel).ToList();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddManga(MangaItem manga)
    {
        //if (!ModelState.IsValid)
        //{
        //    return View(manga);
        //}
        manga.CoverImageUrl = await coverUrlService.SaveCoverUrl(manga.CoverImage, manga.Title, manga.Id);

        await mangaService.CreateManga(MapToMangaBusinessModel(manga));


        TempData["AdminMessage"] = $"Saved: {manga.Title}.";

        return RedirectToAction(nameof(Manga));
    }

    [HttpGet]
    public async Task<IActionResult> EditManga(int id)
    {
        var manga = await mangaService.GetMangaById(id);

        if (manga is null)
        {
            return NotFound();
        }

        return View(MapToMangaViewModel(manga));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteManga([FromBody] MangaItem mangaitem)
    {
        try
        {
            var deleted = await mangaService.DeleteManga(mangaitem.IdforDelete);

            return Json(new
            {
                message = $"Deleted manga with ID: {mangaitem.IdforDelete}"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.ToString());
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditManga(MangaItem manga)
    {
        if (!ModelState.IsValid)
        {
            return View(manga);
        }

        if (manga.CoverImage is not null && manga.CoverImage.Length > 0)
        {
            manga.CoverImageUrl = await coverUrlService.SaveCoverUrl(manga.CoverImage, manga.Title, manga.Id);
        }

        var updated = await mangaService.UpdateManga(MapToMangaBusinessModel(manga));

        if (!updated)
        {
            return NotFound();
        }

        TempData["AdminMessage"] = $"Updated: {manga.Title}.";

        return RedirectToAction(nameof(Manga));
    }

    [HttpGet]
    public async Task<IActionResult> AddChapter(int mangaId)
    {
        var manga = await mangaService.GetMangaById(mangaId);

        if (manga is null)
        {
            return NotFound();
        }

        return View(new ChapterFormViewModel
        {
            MangaId = manga.Id,
            MangaTitle = manga.Title,
            ChapterNumber = manga.Chapters + 1,
            Title = $"Chapter {manga.Chapters + 1}",
            ImageFolder = $"/Mangas/{manga.Title}/{manga.Id}/"
        });
    }

    //chapter{manga.Chapters + 1}/

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> AddChapter(ChapterFormViewModel chapter)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        return View(chapter);
    //    }


    //    var chapterUrl = await chapterUrlService.SaveChapterUrl(
    //        chapter.PageImages,
    //        chapter.MangaTitle,
    //        chapter.MangaId,
    //        chapter.ChapterNumber);
    //    var saved = await mangaService.AddChapter(new BM_ChapterItem
    //    {
    //        MangaId = chapter.MangaId,
    //        ChapterNumber = chapter.ChapterNumber,
    //        Title = chapter.Title,
    //        ChapterUrl = chapterUrl
    //    });

    //    if (!saved)
    //    {
    //        return NotFound();
    //    }

    //    TempData["AdminMessage"] = $"Chapter saved: {chapter.MangaTitle} chapter {chapter.ChapterNumber}";

    //    return RedirectToAction(nameof(Manga));
    //}

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(524288000)] // Extends request allowance to 500MB for batch uploads
    public async Task<IActionResult> AddChapter(ChapterFormViewModel chapter)
    {
        // 1. Structural Fail-Safe Check
        // Bypass strict ModelState.IsValid since the UI form elements for Title/Number are omitted
        if (chapter.MangaId <= 0 || chapter.PageImages == null || !chapter.PageImages.Any())
        {
            ModelState.AddModelError("PageImages", "Target Manga selection or uploaded documents are missing.");
            return View(chapter);
        }

        int successCount = 0;
        List<string> failedFilesList = new List<string>();

        // 2. Loop through every uploaded file
        foreach (var file in chapter.PageImages)
        {
            if (file.Length == 0) continue;

            // 3. Extract the sequence number from filename using Regex
            // Looks for the first contiguous block of numbers (e.g., "Ch_84.pdf" -> 84, "001.pdf" -> 1)
            var match = Regex.Match(file.FileName, @"\d+");

            if (!match.Success)
            {
                failedFilesList.Add($"{file.FileName} (Reason: No number found in filename)");
                continue;
            }

            int extractedChapterNumber = int.Parse(match.Value);
            string fallbackTitle = $"Chapter {extractedChapterNumber}";

            try
            {
                // 4. Save file payload to physical/cloud target paths via your service layer
                var chapterUrl = await chapterUrlService.SaveChapterUrl(
                    file,
                    chapter.MangaTitle,
                    chapter.MangaId,
                    extractedChapterNumber);

                // 5. Commit record to your database Context
                var saved = await mangaService.AddChapter(new BM_ChapterItem
                {
                    MangaId = chapter.MangaId,
                    ChapterNumber = extractedChapterNumber,
                    Title = fallbackTitle,
                    ChapterUrl = chapterUrl
                });

                if (saved)
                {
                    successCount++;
                }
                else
                {
                    failedFilesList.Add($"{file.FileName} (Reason: Database insertion rejected)");
                }
            }
            catch (Exception ex)
            {
                // Capture file stream errors, permission locks, or validation rejections
                failedFilesList.Add($"{file.FileName} (Error: {ex.Message})");
            }
        }

        // 6. Build response state messages
        if (failedFilesList.Any())
        {
            TempData["AdminMessage"] = $"Processed {successCount} chapters successfully. Errors: {string.Join(", ", failedFilesList)}";
        }
        else
        {
            TempData["AdminMessage"] = $"Successfully batched and built {successCount} chapters for '{chapter.MangaTitle}'!";
        }

        // Redirect back to your base dashboard view grid
        return RedirectToAction(nameof(Manga));
    }


    [HttpGet]
    public async Task<IActionResult> EditUser(int id)
    {
        var user = await userService.GetUserById(id);

        if (user is null)
        {
            return NotFound();
        }

        return View(MapToUserViewModel(user));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(AdminUserItem user)
    {
        var updated = await userService.UpdateUser(new BM_UserItem
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role
        });

        if (!updated)
        {
            return NotFound();
        }

        TempData["AdminMessage"] = $"Updated: {user.Name}.";

        return RedirectToAction(nameof(Users));
    }



    private static MangaItem MapToMangaViewModel(BM_MangaItem manga)
    {
        return new MangaItem
        {
            Id = manga.Id,
            Title = manga.Title,
            Author = manga.Author,
            Status = manga.Status,
            Description = manga.Description,
            CoverImageUrl = manga.CoverImage,
            ReleaseYear = manga.ReleaseYear,
            CreatedAt = manga.CreatedAt,
            Genres = manga.Genres,
            GenresText = string.Join(", ", manga.Genres),
            Chapters = manga.Chapters,
            Views = manga.Views,
            Rating = manga.Rating,
            Featured = manga.Featured
        };
    }

    private static BM_MangaItem MapToMangaBusinessModel(MangaItem manga)
    {
        return new BM_MangaItem
        {
            Id = manga.Id,
            Title = manga.Title,
            Author = manga.Author,
            Status = manga.Status,
            Description = manga.Description,
            CoverImage = manga.CoverImageUrl,
            ReleaseYear = manga.ReleaseYear ?? 0,
            CreatedAt = manga.CreatedAt,
            Genres = SplitGenres(manga.GenresText),
            Chapters = manga.Chapters,
            Views = manga.Views,
            Rating = manga.Rating,
            Featured = manga.Featured
        };
    }

    private static AdminUserItem MapToUserViewModel(BM_UserItem user)
    {
        return new AdminUserItem
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            Joined = user.Joined
        };
    }

    private static string[] SplitGenres(string? genres)
    {
        return (genres ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static IEnumerable<MangaItem> FilterManga(IEnumerable<MangaItem> manga, string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return manga;
        }

        var search = query.Trim();

        return manga.Where(item =>
            item.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            item.Author.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            item.Status.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            item.Genres.Any(genre => genre.Contains(search, StringComparison.OrdinalIgnoreCase)));
    }

    public async Task<IActionResult> ExportToExcel()
    {
        var mangas = await mangaService.GetAllManga();

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Mangas");

            // Header
            worksheet.Cell(1, 1).Value = "Id";
            worksheet.Cell(1, 2).Value = "Title";
            worksheet.Cell(1, 5).Value = "Description";
            worksheet.Cell(1, 4).Value = "Status";
            worksheet.Cell(1, 3).Value = "Author";
            worksheet.Cell(1, 6).Value = "Genres";
            int row = 2;

            foreach (var manga in mangas)
            {
                worksheet.Cell(row, 1).Value = manga.Id;
                worksheet.Cell(row, 2).Value = manga.Title;
                worksheet.Cell(row, 3).Value = manga.Author;
                worksheet.Cell(row, 4).Value = manga.Status;
                worksheet.Cell(row, 5).Value = manga.Description;
                worksheet.Cell(row, 6).Value = string.Join(", ", manga.Genres);


                row++;
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);

                var content = stream.ToArray();

                return File(
                    content,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "MangaList.xlsx"
                );

            }
        }
    }

    [HttpPost]
    public async Task<IActionResult> ExcelFileSave(IFormFile excelFile)
    {
        if (excelFile == null || excelFile.Length < 1)
        {
            TempData["AdminMess"] = $"ExcelFile Doesn't Exist!";
            return RedirectToAction(nameof(Index));
        }

        using Stream stream = excelFile.OpenReadStream();

        using (var xlsx = new XLWorkbook(stream))
        {
            var workSheet = xlsx.Worksheets.First();
            var rows = workSheet.RowsUsed().Skip(1);

            foreach (var row in rows)
            {
                // 1. Safely extract values from the row
                string idRaw = row.Cell(1).GetString()?.Trim();
                string title = row.Cell(2).GetString();
                string Author = row.Cell(3).GetString();
                string Status = row.Cell(4).GetString();
                string description = row.Cell(5).GetString();
                string genresText = row.Cell(6).GetString();
                string[] genres = SplitGenres(genresText);

                // 2. Skip completely blank rows that Excel sometimes includes at the bottom
                if (string.IsNullOrEmpty(idRaw) && string.IsNullOrEmpty(title) && string.IsNullOrEmpty(Author))
                {
                    continue;
                }

                BM_MangaItem existingManga = null;

                // 3. Only look up by ID if the ID cell is not empty and is a valid number
                if (!string.IsNullOrEmpty(idRaw) && int.TryParse(idRaw, out int id))
                {
                    existingManga = await mangaService.GetMangaById(id);
                }

                // 4. Update or Insert logic
                if (existingManga != null)
                {
                    // Update existing record
                    existingManga.Title = title;
                    existingManga.Author = Author;
                    existingManga.Status = Status;
                    existingManga.Description = description;
                    existingManga.Genres = genres;

                    await mangaService.UpdateManga(existingManga);
                    TempData["AdminMess"] = $"Updated: Successful";
                }
                else
                {
                    // Insert new record (Database handles auto-increment automatically)
                    BM_MangaItem newBook = new BM_MangaItem
                    {
                        Title = title,
                        Author = Author,
                        Status = Status,
                        Genres = genres,
                        Description = description,
                    };

                    await mangaService.CreateManga(newBook);
                    TempData["AdminMess"] = $"Inserted: Successful";
                }
            }

        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteChapter([FromBody] MangaItem request)

    {

        var manga = await mangaService.GetMangaById(request.Id);
        

        if (manga == null)
        {
            return Json(new { message = "Manga not found" });
        }

        var chapterIdToDelete = await chapterService.GetChapterIDByChapterNumber(request.Id, request.chapterNumber);

        if (chapterIdToDelete <= 0)
        {
            return Json(new { message = "Chapter not found" });
        }
        var isDeleted = await chapterService.DeleteChapter(chapterIdToDelete);
        if (isDeleted)
        {
            return Json(new { message = $"Chapter {request.chapterNumber } is deleted successfully" });
        }

        return Json(new { message = "Error" });
    }

    public static ChapterItem MapToChapterViewModel(BM_ChapterItem bmChapter)
    {
        return new ChapterItem
        {
            ChapterId = bmChapter.ChapterId,
            MangaId = bmChapter.MangaId,
            ChapterNumber = bmChapter.ChapterNumber,
            Title = bmChapter.Title,
            CreatedAt = bmChapter.CreatedAt,
            ChapterUrl = bmChapter.ChapterUrl
        };
    }
}