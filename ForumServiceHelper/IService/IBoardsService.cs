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
        public Task<IList<BoardsViewModel>> GetAllBoardsAsync();
        //public IQueryable<BoardListItemViewModel> GetAllOrSearch(string searchKeyWord);
        //public BoardCreateViewModel? GetDetails(int id);
        public bool CheckAndDelete(int id);
        public bool CheckAndCreate(ForumBoardCategory data);
        public bool CheckAndUpdate(ForumBoardCategory data);

        //public BoardStatsViewModel GetBoardsStatus(); //看板管理數據統計資料

        //public bool UpdateSortAndStatus(List<BatchUpdateViewModel> items); //批次修改
    }
}
