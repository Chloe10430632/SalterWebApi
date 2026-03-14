using ForumRepositoryHelper.IRepository;
using ForumServiceHelper.IService;
using ForumServiceHelper.Models.DTO.Const;
using ForumServiceHelper.Models.DTO.QueryModel;
using ForumServiceHelper.Models.DTO.ViewModel;
using SalterEFModels.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ForumServiceHelper.Service
{
    public class BoardsService : IBoardsService
    {
        private readonly IGenericSalterRepository<ForumBoardCategory> _dbBoards;

        public BoardsService(IGenericSalterRepository<ForumBoardCategory> dbBoards)
        {
            _dbBoards = dbBoards;
        }

        public async Task<IList<BoardsViewModel>> GetAllBoardsAsync(BoardsQueryModel query)
        {
            var boardQuery = _dbBoards.GetAll();
            //使用 Select 進行投影，讓資料庫執行 COUNT 與 Grouping
            var boardsData = boardQuery.Select(b => new BoardsViewModel
            {
                BoardId = b.BoardId,
                BoardTitle = b.Title,
                BoardImgUrl = b.ImageUrl,
                BoardSort = b.SortOrder,
                // 直接利用導覽屬性計算數量，EF Core高效子查詢，每一筆看板資料跑的時候相關互動進行計算
                ViewCount = b.ForumBoardInteractions
                     .Count(i => i.Type == BoardInteractionTypes.View),
                FollowCount = b.ForumBoardInteractions
                      .Count(i => i.Type == BoardInteractionTypes.Follow)
            });

            //處理排序邏輯
            var sortBy = query.SortBy?.Trim().ToUpper();
            boardsData = sortBy switch
            {
                "POPULAR" => boardsData.OrderByDescending(x => x.ViewCount),
                "FOLLOW" => boardsData.OrderByDescending(x => x.FollowCount),
                _ => boardsData.OrderBy(x => x.BoardSort) //預設
            };

            // 回傳筆數
            if (query.TakeSize > 0)
            {
                boardsData = boardsData.Take(query.TakeSize);
            }

            // 執行非同步查詢並回傳結果
            return await boardsData.ToListAsync();
        }

        public async Task<BoardsViewModel?> GetDetailsAsync(int id)
        {
            var query = _dbBoards.GetAll();
           
            var boardData = await query
            .Where(b => b.BoardId == id)
            .Select(b => new BoardsViewModel
            {
                BoardId = b.BoardId,
                BoardTitle = b.Title,
                BoardImgUrl = b.ImageUrl,
                BoardSort = b.SortOrder,
                ViewCount = b.ForumBoardInteractions
                             .Count(i => i.Type == BoardInteractionTypes.View),
                FollowCount = b.ForumBoardInteractions
                              .Count(i => i.Type == BoardInteractionTypes.Follow)
            })
            .FirstOrDefaultAsync(); 

            return boardData;
        }
    }
}
