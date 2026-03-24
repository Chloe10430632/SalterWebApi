using HomeServiceHelper.Service;
using Microsoft.AspNetCore.Mvc;

namespace SalterWebApi.Areas.House.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Tags("房屋租借")]
    public class UploadController : ControllerBase
    {
        private readonly CloudinaryService _cloudinaryService;

        public UploadController(CloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost("images")]
        public async Task<IActionResult> Upload()
        {
            var files = Request.Form.Files; // 直接從 HTTP Form 拿檔案

            if (files == null || files.Count == 0)
            {
                return Ok(new { urls = new List<string>(), message = "No files received" });
            }

            // 轉換成 List 給 Service
            var fileList = files.ToList();
            var urls = await _cloudinaryService.UploadImagesAsync(fileList, "Houses");

            return Ok(new { urls });
        }
    }
}
