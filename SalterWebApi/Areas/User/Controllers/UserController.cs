using Microsoft.AspNetCore.Authorization;
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


        [Authorize]
        [HttpGet("GetUserProfile")]
        public async Task<ActionResult<UserProfileViewModel>> GetUserProfile()
        {
            // 這裡就是你說的「內建屬性」與「暗號」
            var currentUserId = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized(new { message = "無效的憑證" });

            var profile = await _userService.GetUserProfileAsync(int.Parse(currentUserId));

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
        [Authorize]
        [HttpPut("UpdateUserProfile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UserEditViewModel model)
        {

            var currentUserId = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized(new { message = "無效的憑證" });

            model.Id = int.Parse(currentUserId);

            var success = await _userService.UpdateProfileAsync(model);

            if (!success) return BadRequest(new { message = "更新失敗" });

            return Ok(new { message = "更新成功" });

        }

        // DELETE api/<UserController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}



        [HttpPost("UploadUserPicture")]
        public async Task<IActionResult> UploadUserPicture(IFormFile file)
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


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginViewModel Lmodel)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new { message = "帳號密碼格式錯誤" });
            }

            var token = await _userService.LoginAsync(Lmodel);

            if(string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "帳號密碼錯誤" });
            }

            if (token == "WAITING_FOR_EMAIL_VERIFICATION")
            {
                return StatusCode(403, new
                {
                    message = "帳號尚未啟用，請先完成 Email 驗證",
                    status = "NeedVerification" // 讓前端好判斷
                });
            }

            return Ok(new
            {
                token = token,
                message = "登入成功"
            });

        }

        [HttpPost("VerifyRegisterOtp")]
        public async Task<IActionResult> VerifyRegisterOtp([FromBody] UserOtpVerifyViewModel model)
        {
            // 這裡的 model 就會包含前端傳來的 Email 和 Otp
            var result = await _userService.VerifyRegistrationOtpAsync(model.Email, model.Otp);

            if (!result.success) return BadRequest(new { message = "驗證碼錯誤或已過期" });

            return Ok(new { message = "驗證成功，帳號已啟用，請重新登入" });
        }





        [HttpPost("GoogleLogin")]
        public async Task<IActionResult> GoogleLogin([FromBody] UserGoogleLoginViewModel request)
        {
            if (request == null || string.IsNullOrEmpty(request.IdToken))
            {
                return BadRequest(new { message = "無效的 Google 憑證" });
            }

            // 這裡改傳入 request.IdToken
            var token = await _userService.GoogleLoginAsync(request.IdToken);

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { message = "Google 登入驗證失敗" });
            }

            return Ok(new { token = token, message = "Google 登入成功" });
        }


        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            var result = await _userService.SendPasswordResetOtpAsync(email);
            // 為了資安，通常不論有無此 Email 都回傳 Ok，避免被掃描帳號，但專案發表可以回傳錯誤方便演示
            if (!result) return BadRequest(new { message = "發送失敗" });
            return Ok(new { message = "驗證碼已寄出" });
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] UserResetPasswordViewModel model)
        {
            var success = await _userService.ResetPasswordAsync(model.Email, model.Otp, model.NewPassword);
            if (!success) return BadRequest(new { message = "驗證碼錯誤或已過期" });
            return Ok(new { message = "密碼重設成功" });
        }
    }
}
