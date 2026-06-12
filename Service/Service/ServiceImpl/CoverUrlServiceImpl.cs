using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Service.Service.ServiceImpl
{
    public class CoverUrlServiceImpl : CoverUrlService
    {
        public async Task<string> SaveCoverUrl(IFormFile? file, string title, int mangaId)
        {
            title = title.Replace(' ', '_');
            title = Regex.Replace(title, @"[/:*?""<>|]", "_");
            var urlPath = $"/Mangas/{title}/{mangaId}/";
            var filePath = Path.Combine("wwwroot", "Mangas", title, mangaId.ToString());

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            if (file != null && file.Length > 0)
            {
                //var extension = Path.GetExtension(file.FileName);
                var fileName = Path.GetFileName(file.FileName);
                var fullPath = Path.Combine(filePath, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await file.CopyToAsync(stream);
                return urlPath + fileName;
            }
            return string.Empty;

        }

    }
}
