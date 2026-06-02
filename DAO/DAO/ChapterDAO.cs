using Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO.DAO
{
    public interface ChapterDAO
    {
        public Task<List<Chapter>> GetChaptersByMangaId(int mangaId);
        public Task<Chapter?> GetChapterById(int id);
    }
}
