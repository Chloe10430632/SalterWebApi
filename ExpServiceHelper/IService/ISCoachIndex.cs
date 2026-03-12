using ExpRepositoryHelper.Repository;
using ExpServiceHelper.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ExpServiceHelper.DTO.DFavCoach;

namespace ExpServiceHelper.IService
{
    public interface ISCoachIndex
    {
        /**收藏教練*/
        Task<string?> MyFavCoach(DFavCoach a);
        
      
    }
}
