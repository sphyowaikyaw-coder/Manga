using Dependency;
using Microsoft.EntityFrameworkCore;

namespace DAO.DAO.DAOImpl;

public class MangaDAOImpl(MangaDbContext mangaDbContext) : MangaDAO
{
    public async Task<List<Manga>> GetAllManga()
    {
        return await MangaQuery()
            .OrderByDescending(manga => manga.MangaId)
            .ToListAsync();
    }

    public async Task<Manga?> GetMangaById(int id)
    {
        return await MangaQuery()
            .FirstOrDefaultAsync(manga => manga.MangaId == id);
    }

    public async Task<bool> CreateManga(Manga manga)
    {
        await AttachAuthor(manga);
        await AttachGenres(manga);
        await mangaDbContext.Mangas.AddAsync(manga);
        await mangaDbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateManga(Manga manga)
    {
        var existing = await mangaDbContext.Mangas
            .Include(item => item.Author)
            .Include(item => item.MangaGenres)
            .FirstOrDefaultAsync(item => item.MangaId == manga.MangaId);

        if (existing is null)
        {
            return false;
        }

        existing.Title = manga.Title;
        existing.Description = manga.Description;
        existing.CoverImage = manga.CoverImage;
        existing.Status = manga.Status;
        existing.ViewCount = manga.ViewCount;
        existing.Author = manga.Author;
        await AttachAuthor(existing);
        await ReplaceGenres(existing, manga.MangaGenres);

        await mangaDbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddChapter(Chapter chapter)
    {
        var mangaExists = await mangaDbContext.Mangas.AnyAsync(manga => manga.MangaId == chapter.MangaId);

        if (!mangaExists)
        {
            return false;
        }

        await mangaDbContext.Chapters.AddAsync(chapter);
        await mangaDbContext.SaveChangesAsync();
        return true;
    }

    private IQueryable<Manga> MangaQuery()
    {
        return mangaDbContext.Mangas
            .AsNoTracking()
            .Include(manga => manga.Author)
            .Include(manga => manga.Chapters)
            .Include(manga => manga.Ratings)
            .Include(manga => manga.MangaGenres)
                .ThenInclude(mangaGenre => mangaGenre.Genre);
    }

    private async Task AttachAuthor(Manga manga)
    {
        var authorName = manga.Author?.AuthorName?.Trim();

        if (string.IsNullOrWhiteSpace(authorName))
        {
            manga.Author = null;
            manga.AuthorId = null;
            return;
        }

        var existingAuthor = await mangaDbContext.Authors
            .FirstOrDefaultAsync(author => author.AuthorName == authorName);

        if (existingAuthor is null)
        {
            manga.Author = new Author { AuthorName = authorName };
            manga.AuthorId = null;
            return;
        }

        manga.Author = existingAuthor;
        manga.AuthorId = existingAuthor.AuthorId;
    }

    private async Task AttachGenres(Manga manga)
    {
        var genreNames = GetGenreNames(manga.MangaGenres);
        manga.MangaGenres.Clear();

        foreach (var genreName in genreNames)
        {
            manga.MangaGenres.Add(new MangaGenre
            {
                Genre = await FindOrCreateGenre(genreName)
            });
        }
    }

    private async Task ReplaceGenres(Manga manga, IEnumerable<MangaGenre> mangaGenres)
    {
        var genreNames = GetGenreNames(mangaGenres);
        mangaDbContext.MangaGenres.RemoveRange(manga.MangaGenres);
        manga.MangaGenres.Clear();

        foreach (var genreName in genreNames)
        {
            manga.MangaGenres.Add(new MangaGenre
            {
                MangaId = manga.MangaId,
                Genre = await FindOrCreateGenre(genreName)
            });
        }
    }

    private async Task<Genre> FindOrCreateGenre(string genreName)
    {
        var existingGenre = await mangaDbContext.Genres
            .FirstOrDefaultAsync(genre => genre.GenreName == genreName);

        return existingGenre ?? new Genre { GenreName = genreName };
    }

    private static string[] GetGenreNames(IEnumerable<MangaGenre> mangaGenres)
    {
        return mangaGenres
            .Select(mangaGenre => mangaGenre.Genre?.GenreName)
            .Where(genreName => !string.IsNullOrWhiteSpace(genreName))
            .Select(genreName => genreName!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
