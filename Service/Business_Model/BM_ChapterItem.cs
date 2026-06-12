namespace Service.Business_Model;

public class BM_ChapterItem
{
    public int ChapterId { get; set; }

    public int MangaId { get; set; }

    public decimal ChapterNumber { get; set; }

    public string Title { get; set; } = string.Empty;

    public DateTime? CreatedAt { get; set; }

    public string ChapterUrl { get; set; } = string.Empty;
}
