using ForumRepositoryHelper.IRepository;
using ForumServiceHelper.IService;
using ForumServiceHelper.Models.DTO.Const;
using ForumServiceHelper.Models.DTO.ViewModel;
using SalterEFModels.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumServiceHelper.Service
{
    public class BoardsService : IBoardsService
    {
        private readonly IGenericSalterRepository<ForumBoardCategory> _dbBoards;

        public BoardsService(IGenericSalterRepository<ForumBoardCategory> dbBoards)
        {
            _dbBoards = dbBoards;
        }

        public async Task<IList<BoardsViewModel>> GetAllBoardsAsync(string sortBy = "DEFAULT")
        {
            var boards = await _dbBoards.GetAllAsync();
            var boardsViewCount = _dbBoards.GetDbContext().ForumBoardInteractions
                .Where(bi => bi.Type == BoardInteractionTypes.View)
                .GroupBy(p => p.BoardId)
                .Select(g => new { BoardId = g.Key, Count = g.Count() });

            var boardsFollowCount = _dbBoards.GetDbContext().ForumBoardInteractions
                .Where(bi => bi.Type == BoardInteractionTypes.Follow)
                .GroupBy(p => p.BoardId)
                .Select(g => new { BoardId = g.Key, Count = g.Count() });

            var boardList = boards
                .Select(b => new BoardsViewModel
                {
                    BoardId = b.BoardId,
                    BoardTitle = b.Title,
                    BoardImgUrl = b.ImageUrl,
                    BoardSort = b.SortOrder,
                    ViewCount = boardsViewCount
                    .Where(v => v.BoardId == b.BoardId)
                    .Select(v => v.Count)
                    .FirstOrDefault(),
                    FollowCount = boardsFollowCount
                     .Where(f => f.BoardId == b.BoardId)
                    .Select(f => f.Count)
                    .FirstOrDefault(),
                });

            sortBy = sortBy.Trim().ToUpper();

            if(sortBy == SortTypes.Default)
            {
                boardList = boardList.OrderBy(bv => bv.BoardSort);
            }
            
            if (sortBy == SortTypes.Popular)
            {
                boardList = boardList.OrderByDescending(bv => bv.ViewCount);
            }

            if (sortBy == SortTypes.Follow)
            {
                boardList = boardList.OrderByDescending(bv => bv.FollowCount);
            }

            return boardList.ToList();

        }

        public async Task<BoardsViewModel?> GetDetailsAsync(int id)
        {
            var board = await _dbBoards
                .GetTableByIDAsync(id);
            var boardsViewCount = _dbBoards.GetDbContext().ForumBoardInteractions
               .Where(bi => bi.Type == BoardInteractionTypes.View)
               .GroupBy(p => p.BoardId)
               .Select(g => new { BoardId = g.Key, Count = g.Count() });
            var boardsFollowCount = _dbBoards.GetDbContext().ForumBoardInteractions
               .Where(bi => bi.Type == BoardInteractionTypes.Follow)
               .GroupBy(p => p.BoardId)
               .Select(g => new { BoardId = g.Key, Count = g.Count() });

            if (board == null)
                return  null;

            var boardData = new BoardsViewModel
            {
                BoardId = board.BoardId,
                BoardImgUrl = board.ImageUrl,
                BoardSort = board.SortOrder,
                BoardTitle = board.Title,
                ViewCount = boardsViewCount
                    .Where(v => v.BoardId == board.BoardId)
                    .Select(v => v.Count)
                    .FirstOrDefault(),
                FollowCount = boardsFollowCount
                     .Where(f => f.BoardId == board.BoardId)
                    .Select(f => f.Count)
                    .FirstOrDefault()
            };

            return boardData;
        }
    }
}
