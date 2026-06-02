using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WebApp.View_Model;

public class ChapterFormViewModel
{
    public int MangaId { get; set; }

    public string MangaTitle { get; set; } = string.Empty;

    [Required]
    [Range(1, 9999)]
    public int ChapterNumber { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    public string ImageFolder { get; set; } = string.Empty;

    public IFormFile? PageImages { get; set; }
}