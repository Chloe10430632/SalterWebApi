using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpRepositoryHelper;
using ExpRepositoryHelper.IRepository;
using ExpServiceHelper.IService;
using SalterEFModels.EFModels;
using ExpServiceHelper.DTO;
using static ExpServiceHelper.DTO.DCoachIndex;
using ExpRepositoryHelper.Repository;


namespace ExpServiceHelper.Service
{
    internal class SCoachIndex : ISCoachIndex
    {
        private readonly IRCoachIndex _rCoachIndex;
        public SCoachIndex(IRCoachIndex rCoachIndex) { _rCoachIndex = rCoachIndex; }
        
        public async Task<string?> MyFavCoach(FavCoachCreateDto a)
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
        public Task<(List<RCoachIndex> Items, int TotalCount)> DistrictAsync(string? dist, string? sortBy, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<(List<RCoachIndex> Items, int TotalCount)> HOTAsync(string? Hot, string? sortBy, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

       
    }
}
