using System;
using System.Collections.Generic;

namespace Dependency;

public partial class ReadingHistory
{
    public int HistoryId { get; set; }

    public int? UserId { get; set; }

    public int? MangaId { get; set; }

    public int? ChapterId { get; set; }

    public DateTime? LastReadAt { get; set; }

    public virtual Chapter? Chapter { get; set; }

    public virtual Manga? Manga { get; set; }

    public virtual User? User { get; set; }
}
