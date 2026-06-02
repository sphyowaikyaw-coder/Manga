using System;
using System.Collections.Generic;

namespace Dependency;

public partial class Chapter
{
    public int ChapterId { get; set; }

    public int MangaId { get; set; }

    public decimal? ChapterNumber { get; set; }

    public string? Title { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? ChapterUrl { get; set; }

    public virtual Manga Manga { get; set; } = null!;

    public virtual ICollection<ReadingHistory> ReadingHistories { get; set; } = new List<ReadingHistory>();
}
