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
using static ExpServiceHelper.DTO.DCoachFav;
using System.Security.Claims;


namespace ExpServiceHelper.Service
{
    public class SCoachIndex : ISCoachIndex
    {
        private readonly IRCoachIndex _rCoachIndex;
        public SCoachIndex(IRCoachIndex rCoachIndex) { _rCoachIndex = rCoachIndex; }
        
        public async Task<string?> MyFavCoach(DCoachFav a, int UserId)
        {
            try {//用傳進來的 userId//
            // 1.先檢查
            var isExistde = await _rCoachIndex.ExistAsync(UserId, a.CoachId);
            // 2.取消收藏
            if (isExistde)
            {
                await _rCoachIndex.DeleteFavCoach(UserId, a.CoachId);
                return "取消收藏 QAQ";
            }
            //  3.收藏
            else
            {
                var entity = new ExpFavorite
                {
                    UserId = UserId,
                    CoachId = a.CoachId,
                    FavoritedAt = DateTime.Now
                };

                await _rCoachIndex.AddFavCoach(entity);
                return "收藏！";
            } }
            catch (Exception ex) { throw new Exception($"{ex.Message}"); }
        }
        
    }
}
