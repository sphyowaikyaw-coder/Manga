using Dependency;

namespace DAO.DAO;

public interface MangaDAO
{
    Task<List<Manga>> GetAllManga();

    Task<Manga?> GetMangaById(int id);

    Task<bool> CreateManga(Manga manga);

    Task<bool> UpdateManga(Manga manga);

    Task<bool> AddChapter(Chapter chapter);

    Task<bool> DeleteManga(int id);
}
