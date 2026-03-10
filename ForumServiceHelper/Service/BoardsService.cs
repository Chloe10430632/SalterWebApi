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
        public bool CheckAndCreate(ForumBoardCategory data)
        {
            throw new NotImplementedException();
        }

        public bool CheckAndDelete(int id)
        {
            throw new NotImplementedException();
        }

        public bool CheckAndUpdate(ForumBoardCategory data)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<BoardsViewModel>> GetAllBoardsAsync()
        {
            var boards = await _dbBoards.GetAllAsync();
            var boardsViewCount = _dbBoards.GetDbContext().ForumBoardInteractions
                .Where(bi => bi.Type == BoardInteractionTypes.View)
                .GroupBy(p => p.BoardId)
                .Select(g => new { BoardId = g.Key, Count = g.Count() });

            var board = boards
                .Select(b => new BoardsViewModel
                {
                    BoardId = b.BoardId,
                    BoardTitle = b.Title,
                    BoardImgUrl = b.ImageUrl,
                    BoardSort = b.SortOrder,
                    ViewCount = boardsViewCount
                    .Where(v => v.BoardId == b.BoardId)
                    .Select(v=>v.Count)
                    .FirstOrDefault()
                })
                .OrderBy(bv => bv.BoardSort)
               // .OrderByDescending(bv => bv.ViewCount)
                .ToList();

            return board;
        }
    }
}
