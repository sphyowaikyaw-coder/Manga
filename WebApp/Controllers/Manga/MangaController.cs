using Microsoft.AspNetCore.Mvc;
using Service.Business_Model;
using Service.Service;
using WebApp.View_Model;

namespace WebApp.Controllers.Manga;

public class MangaController(MangaService mangaService, ChapterUrlService chapterUrlService, ChapterService chapterService) : Controller
{
    public async Task<IActionResult> Index(string? q = null, bool showAllNew = false)
    {
        var manga = await mangaService.GetAllManga();
        var viewModel = manga.Select(MapToViewModel).ToList();

        viewModel = FilterManga(viewModel, q).ToList();
        ViewData["SearchQuery"] = q ?? string.Empty;
        ViewData["ShowAllNew"] = showAllNew;

        return View("Views/Manga/MangaView.cshtml", viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var manga = await mangaService.GetMangaById(id);
       
        if (manga is null)
        {
            return NotFound();
        }

        return View("Views/Manga/Details.cshtml", MapToViewModel(manga));
    }

    public async Task<IActionResult> Read(int id, int chapter)
    {
        var manga = await mangaService.GetMangaById(id);
        MangaItem mangaItem = new MangaItem();

        if (manga is null)
        {
            return NotFound();
        }
        var chapters = await chapterService.GetChaptersByMangaId(id);
        if (chapters is null)
        {
                        return NotFound();
        }
        else
        {
            foreach (var item in chapters)
            {
                if (item.ChapterNumber == chapter)
                {
                    mangaItem.Chapter = new ChapterItem
                    {
                       
                        ChapterNumber = item.ChapterNumber,
                        Title = item.Title,
                        ChapterUrl = item.ChapterUrl
                    };
                    break;
                }
            }
        }
            var viewModel = MapToViewModel(manga);
            viewModel.Chapter = new ChapterItem
            {
                ChapterNumber = mangaItem.Chapter?.ChapterNumber ?? 0,
                Title = mangaItem.Chapter?.Title ?? "Chapter not found",
                ChapterUrl = mangaItem.Chapter?.ChapterUrl ?? string.Empty
            };
        var ch = Math.Clamp(chapter, 1, Math.Max(1, viewModel.Chapters));
        Console.WriteLine($"Requested chapter: {chapter}, Clamped chapter: {ch}");
        ViewData["Chapter"] = ch;

        return View("Views/Manga/Read.cshtml", viewModel);
    }



    public static MangaItem MapToViewModel(BM_MangaItem manga)
    {
        return new MangaItem
        {
            Id = manga.Id,
            Title = manga.Title,
            Author = manga.Author,
            Status = manga.Status,
            Description = manga.Description,
            CoverImageUrl = manga.CoverImage,
            Genres = manga.Genres,
            Chapters = manga.Chapters,
            Views = manga.Views,
            Rating = manga.Rating,
            Featured = manga.Featured
        };
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
}
