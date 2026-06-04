using Service.Business_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public interface ChapterService
    {
        public Task<List<BM_ChapterItem>> GetChaptersByMangaId(int mangaId);
        public Task<BM_ChapterItem?> GetChapterById(int id);

        public Task<bool> DeleteChapter(int id);

        public Task<int> GetChapterIDByChapterNumber(int mangaId, int chapterNumber);
    }
}
