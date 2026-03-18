using ForumRepositoryHelper.IRepository;
using ForumServiceHelper.IService;
using ForumServiceHelper.Models.DTO.ViewModel;
using Microsoft.EntityFrameworkCore;
using SalterEFModels.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumServiceHelper.Service
{
    public class AdsService : IAdsService
    {
        private readonly IGenericSalterRepository<ForumAd> _dbAds;

        public AdsService(IGenericSalterRepository<ForumAd> dbAds)
        {
            _dbAds = dbAds;
        }



        public async Task<AdsViewModel?> GetActiveAdAsync()
        {
            DateTime now = DateTime.Now;

            var ad = await _dbAds.GetAll()
               .Where(a => a.Status == "ACTIVE" && a.StartDate <= now && a.EndDate >= now)
               .Select(a=>new AdsViewModel
               {
                   Id = a.AdId,
                   ImageUrl = a.ImgUrl,
                   TargetUrl = a.LinkUrl,
                   TooltipText = a.Title
               })
                .FirstOrDefaultAsync();

            if(ad == null)
                return null;

            return ad;
        }
    }
}
