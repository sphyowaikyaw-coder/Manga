using Microsoft.AspNetCore.Http;

namespace Service.Service.ServiceImpl;

public class ChapterUrlServiceImpl : ChapterUrlService
{
    public async Task<string> SaveChapterUrl(IFormFile file,string title, int mangaId, int chapterNumber)
    {
        title = title.Replace(' ', '_');
        var urlPath = $"/Mangas/{title}/{mangaId}/";
        var filePath = Path.Combine("wwwroot", "Mangas",title, mangaId.ToString());

        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }

        if (file != null && file.Length > 0)
        {
            //var extension = Path.GetExtension(file.FileName);
            var fileName = Path.GetFileName(file.FileName);
            var fullPath = Path.Combine(filePath, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);
            return urlPath + fileName;
        }
        return string.Empty;

    }

   
}
