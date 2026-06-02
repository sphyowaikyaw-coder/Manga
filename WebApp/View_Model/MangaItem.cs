namespace WebApp.View_Model;

public class MangaItem
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public IFormFile? CoverImage { get; set; }

    public string CoverImageUrl { get; set; } = string.Empty;

    public string[] Genres { get; set; } = [];

    public string GenresText { get; set; } = string.Empty;

    public int Chapters { get; set; }

    public int Views { get; set; }

    public decimal Rating { get; set; }

    public bool Featured { get; set; }

    public ChapterItem? Chapter { get; set; }
}
