using Service.Business_Model;

namespace Service.Service;

public interface MangaService
{
    Task<List<BM_MangaItem>> GetAllManga();

    Task<BM_MangaItem?> GetMangaById(int id);

    Task<bool> CreateManga(BM_MangaItem manga);

    Task<bool> UpdateManga(BM_MangaItem manga);

    Task<bool> AddChapter(BM_ChapterItem chapter);
}
