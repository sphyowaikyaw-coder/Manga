using System;
using System.Collections.Generic;

namespace Dependency;

public partial class Rating
{
    public int RatingId { get; set; }

    public int? UserId { get; set; }

    public int? MangaId { get; set; }

    public int? RatingValue { get; set; }

    public virtual Manga? Manga { get; set; }

    public virtual User? User { get; set; }
}
