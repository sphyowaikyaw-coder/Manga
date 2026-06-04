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
        public async Task<bool> DeleteChapter(int id)
        {
            var chapter = await mangaDbContext.Chapters.FindAsync(id);
            if (chapter == null)
            {
                return false;
            }
    
            mangaDbContext.Chapters.Remove(chapter);
            await mangaDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetChapterIDByChapterNumber(int mangaId, int chapterNumber)  
        {
            var chapter = await mangaDbContext.Chapters
                .Where(ch => ch.MangaId == mangaId && ch.ChapterNumber == chapterNumber)
                .Select(ch => new { ch.ChapterId })
                .FirstOrDefaultAsync();
            return chapter != null ? chapter.ChapterId : -1; // Return -1 if not found
        }


    }
}
