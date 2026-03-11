using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using UserServiceHelper.IService;
using UserServiceHelper.Models.DTO.ViewModel;
using UserServiceHelper.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SalterWebApi.Areas.User.Controllers
{
    [Area("User")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Tags("會員相關")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IFileService _fileService;

        public UserController(IUserService userService, IFileService fileService)
        {
            _userService = userService;
            _fileService = fileService;
        }

        // GET: api/<UserController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<UserController>/5

        [HttpGet("GetUserProfile/{id}")]
        public async Task<ActionResult<UserProfileViewModel>> Get(int id)
        {
            var profile = await _userService.GetUserProfileAsync(id);

            if (profile == null)
                return NotFound(new { message = "找不到該會員資料" });

            return Ok(profile);
        }

        // POST api/<UserController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        // PUT api/<UserController>/5

        [HttpPut("UpdateUserProfile/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UserEditViewModel model)
        {
            if(id != model.Id)
            {
                return BadRequest(new { message = "資料ID不符" });
            }


            var success = await _userService.UpdateProfileAsync(model);

            if (!success)
            {
                return BadRequest(new { message = "更新失敗，請檢查資料" });
            }

            return Ok(new { message = "更新成功" });

        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpPost("UploadUserPicture")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            // 呼叫 Service，指定存到 admin/imgs，它會回傳像 "/admin/imgs/20260311_xxx.jpg" 的路徑
            var path = await _fileService.UploadImageAsync(file, "admin/imgs");

            if (string.IsNullOrEmpty(path))
            {
                return BadRequest(new { message = "請選擇有效的檔案進行上傳" });
            }

            // 回傳路徑給前端 (Angular)，前端之後註冊時再把這個 path 帶入 RegisterViewModel
            return Ok(new { path });
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterViewModel Rmodel)
        {
            if (Rmodel == null)
                return BadRequest(new { message = "資料格式錯誤" });

            var result = await _userService.RegisterAsync(Rmodel);

            if (!result)
            {
                return BadRequest(new { message = "註冊失敗，該 Email 已被使用" });
            }

            return Ok(new { message = "註冊成功" });
                


        }

    }
}
