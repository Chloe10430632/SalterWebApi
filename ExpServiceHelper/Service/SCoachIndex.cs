using ExpRepositoryHelper;
using ExpRepositoryHelper.IRepository;
using ExpRepositoryHelper.Repository;
using ExpServiceHelper.DTO;
using ExpServiceHelper.IService;
using SalterEFModels.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ExpServiceHelper.DTO.DFavCoach;


namespace ExpServiceHelper.Service
{
    public class SCoachIndex : ISCoachIndex
    {
        private readonly IRCoachIndex _rCoachIndex;
        public SCoachIndex(IRCoachIndex rCoachIndex) { _rCoachIndex = rCoachIndex; }
        
        public async Task<string?> MyFavCoach(DFavCoach a)
        {
            // 1.先檢查
            var isExistde = await _rCoachIndex.ExistAsync(a.UserId, a.CoachId);
            // 2.取消收藏
            if (isExistde)
            {
                await _rCoachIndex.DeleteFavCoach(a.UserId, a.CoachId);
                return "取消收藏 QAQ";
            }
            //  3.收藏
            else
            {
                var entity = new ExpFavorites
                {
                    UserId = a.UserId,
                    CoachId = a.CoachId,
                };

                await _rCoachIndex.AddFavCoach(entity);
                return "收藏！";
            }
        }
    }
}
