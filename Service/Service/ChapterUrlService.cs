using Microsoft.AspNetCore.Http;

namespace Service.Service;

public interface ChapterUrlService
{
    Task<string> SaveChapterUrl(IFormFile file,string title, int mangaId, int chapterNumber);
}
