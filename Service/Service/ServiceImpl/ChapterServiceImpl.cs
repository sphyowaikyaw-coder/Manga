using DAO.DAO;
using Dependency;
using Service.Business_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service.ServiceImpl
{
    public class ChapterServiceImpl(ChapterDAO chapterDAO) : ChapterService
    {
        public async Task<List<BM_ChapterItem>> GetChaptersByMangaId(int mangaId)
        {
            var chapters = await chapterDAO.GetChaptersByMangaId(mangaId);
            return chapters.Select(MapToBusinessModel).ToList();
        }
        public async Task<BM_ChapterItem?> GetChapterById(int id)
        {
            var chapter = await chapterDAO.GetChapterById(id);
            return chapter is null ? null : MapToBusinessModel(chapter);
        }

        private BM_ChapterItem? MapToBusinessModel(Chapter chapter)
        {
            return new BM_ChapterItem
            {
                MangaId = chapter.MangaId,
                ChapterNumber = chapter.ChapterNumber ?? 0,
                Title = chapter.Title ?? string.Empty,
                ChapterUrl = chapter.ChapterUrl ?? string.Empty
            };
        }

       
    }

}
