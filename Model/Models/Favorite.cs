using System;
using System.Collections.Generic;

namespace Dependency;

public partial class Favorite
{
    public int FavoriteId { get; set; }

    public int? UserId { get; set; }

    public int? MangaId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Manga? Manga { get; set; }

    public virtual User? User { get; set; }
}
