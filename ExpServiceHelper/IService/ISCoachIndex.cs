using ExpRepositoryHelper.Repository;
using static ExpServiceHelper.DTO.DCoachIndex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpServiceHelper.IService
{
    public interface ISCoachIndex
    {
        /**收藏教練*/
        Task<string?> MyFavCoach(FavCoachCreateDto a);
        /**搜尋-地區*/
        Task<(List<RCoachIndex> Items, int TotalCount)> DistrictAsync(
         string? dist,
         string? sortBy,
         int page,
         int pageSize);
        /**搜尋-專業*/

        /**排序-最新*/

        /**排序-熱門*/
        Task<(List<RCoachIndex> Items, int TotalCount)> HOTAsync(
          string? Hot,
          string? sortBy,
          int page,
          int pageSize);
    }
}
