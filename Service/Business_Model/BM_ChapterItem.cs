namespace Service.Business_Model;

public class BM_ChapterItem
{
    public int MangaId { get; set; }

    public decimal ChapterNumber { get; set; }

    public string Title { get; set; } = string.Empty;

    public string ChapterUrl { get; set; } = string.Empty;
}
