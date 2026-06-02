using System;
using System.Collections.Generic;

namespace Dependency;

public partial class Author
{
    public int AuthorId { get; set; }

    public string AuthorName { get; set; } = null!;

    public string? Bio { get; set; }

    public string? AuthorImage { get; set; }

    public virtual ICollection<Manga> Mangas { get; set; } = new List<Manga>();
}
