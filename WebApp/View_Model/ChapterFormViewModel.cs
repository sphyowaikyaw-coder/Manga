//using System.ComponentModel.DataAnnotations;
//using Microsoft.AspNetCore.Http;

//namespace WebApp.View_Model;

//public class ChapterFormViewModel
//{
//    public int MangaId { get; set; }

//    public string MangaTitle { get; set; } = string.Empty;

//    [Required]
//    [Range(1, 9999)]
//    public int ChapterNumber { get; set; }

//    [Required]
//    [StringLength(200)]
//    public string Title { get; set; } = string.Empty;

//    public string ImageFolder { get; set; } = string.Empty;

//    public IFormFile? PageImages { get; set; }
//}

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace WebApp.View_Model;

public class ChapterFormViewModel
{
    public int MangaId { get; set; }

    public string MangaTitle { get; set; } = string.Empty;

    // Made optional ([Required] removed) so it doesn't break during multi-file bulk uploads
    [Range(1, 9999)]
    public int? ChapterNumber { get; set; }

    // Made optional so bulk files can fall back to using filenames for titles
    [StringLength(200)]
    public string? Title { get; set; }

    public string ImageFolder { get; set; } = string.Empty;

    // Changed to IList to accept 1 or many files simultaneously
    [Required(ErrorMessage = "Please select at least one file.")]
    public IList<IFormFile> PageImages { get; set; } = new List<IFormFile>();
}
