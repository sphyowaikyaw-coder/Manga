using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public interface CoverUrlService
    {
        public Task<string> SaveCoverUrl(Microsoft.AspNetCore.Http.IFormFile? file, string title, int mangaId);
    }
}
