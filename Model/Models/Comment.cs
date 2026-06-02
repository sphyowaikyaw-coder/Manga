using System;
using System.Collections.Generic;

namespace Dependency;

public partial class Comment
{
    public int CommentId { get; set; }

    public int? UserId { get; set; }

    public int? MangaId { get; set; }

    public string? CommentText { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Manga? Manga { get; set; }

    public virtual User? User { get; set; }
}
