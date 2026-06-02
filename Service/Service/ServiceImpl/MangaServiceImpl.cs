using DAO.DAO;
using Dependency;
using Service.Business_Model;

namespace Service.Service.ServiceImpl;

public class MangaServiceImpl(MangaDAO mangaDAO) : MangaService
{
    public async Task<List<BM_MangaItem>> GetAllManga()
    {
        var manga = await mangaDAO.GetAllManga();

        return manga.Select(MapToBusinessModel).ToList();
    }

    public async Task<BM_MangaItem?> GetMangaById(int id)
    {
        var manga = await mangaDAO.GetMangaById(id);

        return manga is null ? null : MapToBusinessModel(manga);
    }

    public async Task<bool> CreateManga(BM_MangaItem manga)
    {
        return await mangaDAO.CreateManga(MapToDatabaseModel(manga));
    }

    public async Task<bool> UpdateManga(BM_MangaItem manga)
    {
        return await mangaDAO.UpdateManga(MapToDatabaseModel(manga));
    }

    public async Task<bool> AddChapter(BM_ChapterItem chapter)
    {
        return await mangaDAO.AddChapter(new Chapter
        {
            MangaId = chapter.MangaId,
            ChapterNumber = chapter.ChapterNumber,
            Title = chapter.Title,
            ChapterUrl = chapter.ChapterUrl,
            CreatedAt = DateTime.Now
        });
    }

    private static BM_MangaItem MapToBusinessModel(Manga manga)
    {
        var ratingValues = manga.Ratings
            .Where(rating => rating.RatingValue.HasValue)
            .Select(rating => rating.RatingValue!.Value)
            .ToList();
        var genres = manga.MangaGenres
            .Select(mangaGenre => mangaGenre.Genre?.GenreName)
            .Where(genreName => !string.IsNullOrWhiteSpace(genreName))
            .Select(genreName => genreName!)
            .ToArray();

        return new BM_MangaItem
        {
            Id = manga.MangaId,
            Title = manga.Title,
            Author = manga.Author?.AuthorName ?? string.Empty,
            Status = manga.Status ?? string.Empty,
            Description = manga.Description ?? string.Empty,
            CoverImage = string.IsNullOrWhiteSpace(manga.CoverImage)
                ? "/Design/img/trending/trend-1.jpg"
                : manga.CoverImage,
            //HeroImage = string.IsNullOrWhiteSpace(manga.CoverImage)
            //    ? "/Design/img/hero/hero-1.jpg"
            //    : manga.CoverImage,
            Genres = genres,
            Chapters = manga.Chapters.Count,
            Views = manga.ViewCount ?? 0,
            Rating = ratingValues.Count == 0 ? 0 : Math.Round((decimal)ratingValues.Average(), 1),
            Featured = false
        };
    }

    private static Manga MapToDatabaseModel(BM_MangaItem manga)
    {
        return new Manga
        {
            MangaId = manga.Id,
            Title = manga.Title,
            Author = string.IsNullOrWhiteSpace(manga.Author)
                ? null
                : new Author { AuthorName = manga.Author },
            Description = manga.Description,
            CoverImage = 
            //manga.HeroImage
             manga.CoverImage,
            Status = manga.Status,
            ViewCount = manga.Views,
            CreatedAt = DateTime.Now,
            MangaGenres = manga.Genres
                .Where(genre => !string.IsNullOrWhiteSpace(genre))
                .Select(genre => new MangaGenre
                {
                    Genre = new Genre { GenreName = genre.Trim() }
                })
                .ToList()
        };
    }
}
