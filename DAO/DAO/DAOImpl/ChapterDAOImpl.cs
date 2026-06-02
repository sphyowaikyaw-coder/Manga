using Dependency;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO.DAO.DAOImpl
{
    public class ChapterDAOImpl(MangaDbContext mangaDbContext) : ChapterDAO
    {
            public async Task<List<Chapter>> GetChaptersByMangaId(int mangaId)
            {
                return await mangaDbContext.Chapters
                    .Where(chapter => chapter.MangaId == mangaId)
                    .OrderBy(chapter => chapter.ChapterNumber)
                    .ToListAsync();
            }
    
            public async Task<Chapter?> GetChapterById(int id)
            {
                return await mangaDbContext.Chapters
                    .FirstOrDefaultAsync(chapter => chapter.ChapterId == id);
        }
    }
}
