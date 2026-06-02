using System;
using System.Collections.Generic;

namespace Dependency;

public partial class MangaGenre
{
    public int MangaGenreId { get; set; }

    public int? MangaId { get; set; }

    public int? GenreId { get; set; }

    public virtual Genre? Genre { get; set; }

    public virtual Manga? Manga { get; set; }
}
