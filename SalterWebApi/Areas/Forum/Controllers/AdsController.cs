using ForumServiceHelper.IService;
using ForumServiceHelper.Models.DTO.ViewModel;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SalterWebApi.Areas.Forum.Controllers
{
    [Area("Forum")]
    [Route("api/[area]/[controller]")] // 路由 api/Forum/Posts
    [ApiController]
    [Tags("社群討論版")]
    public class AdsController : ControllerBase
    {
        private readonly IAdsService _adsService;

        public AdsController(IAdsService adsService)
        {
            _adsService = adsService;
        }


        // GET: api/<AdsController>
        [HttpGet]
        public async Task<ActionResult<AdsViewModel>> Get()
        {
           var ad =  await _adsService.GetActiveAdAsync();

            if (ad == null)
                throw new KeyNotFoundException("找不到可以上架的廣告");

            return Ok(ad);
        }

    }
}
