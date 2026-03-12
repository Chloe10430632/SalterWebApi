using ExpServiceHelper.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpServiceHelper.IService
{
    public interface ISCoachMethods
    {
        /**搜尋-地區*/
        Task<List<DCoachInfo>> GetCoachDist(string keyDistrict);

        /**搜尋-專業*/

        /**排序-最新*/

        /**排序-熱門*/
    }
}
