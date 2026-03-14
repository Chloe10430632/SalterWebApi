using ForumServiceHelper.Models.DTO.QueryModel;
using ForumServiceHelper.Models.DTO.ViewModel;
using SalterEFModels.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumServiceHelper.IService
{
    public interface IBoardsService
    {
        public Task<IList<BoardsViewModel>> GetAllBoardsAsync(BoardsQueryModel query);
        public Task<BoardsViewModel?> GetDetailsAsync(int id);
        //public IQueryable<BoardListItemViewModel> GetAllOrSearch(string searchKeyWord);

        //public BoardStatsViewModel GetBoardsStatus(); //看板管理數據統計資料

        //public bool UpdateSortAndStatus(List<BatchUpdateViewModel> items); //批次修改
    }
}
