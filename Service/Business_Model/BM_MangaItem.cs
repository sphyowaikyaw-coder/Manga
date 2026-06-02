namespace Service.Business_Model;

public class BM_MangaItem
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string CoverImage { get; set; } = string.Empty;

    public string[] Genres { get; set; } = [];

    public int Chapters { get; set; }

    public int Views { get; set; }

    public decimal Rating { get; set; }

    public bool Featured { get; set; }
}
