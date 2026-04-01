using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Claims;
using UserServiceHelper.IService;
using UserServiceHelper.Models.DTO.ViewModel;
using UserServiceHelper.Service;
using Google.GenAI;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        private readonly string _apiKey;
        private readonly string _systemInstruction;

        public UserController(IUserService userService, IFileService fileService, IConfiguration config)
        {
            _userService = userService;
            _fileService = fileService;

            _apiKey = config["Gemini:ApiKey"] ?? throw new ArgumentNullException("Gemini API Key 沒設定好喔！");

            // 讀取你的 instruction.txt (確保檔案放在專案根目錄)
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "instruction.txt");
            _systemInstruction = System.IO.File.ReadAllText(filePath);
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
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

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

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized(new { message = "無效的憑證" });

            model.Id = int.Parse(currentUserId);

            var newToken = await _userService.UpdateProfileAsync(model);

            if (newToken == null)
            {
                // 這裡就是你說的，失敗的訊息由後端給個基本提示，前端再決定怎麼顯示
                return BadRequest(new { message = "資料更新失敗，請檢查輸入內容" });
            }

            return Ok(new
            {
                token = newToken,
                message = "個人資料已成功更新！"
            });

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
        public async Task<IActionResult> Register([FromBody] UserRegisterViewModel model)
        {
            if (model == null)
                return BadRequest(new { message = "資料格式錯誤" });

            var result = await _userService.RegisterAsync(model);

            if (!result)
            {
                return BadRequest(new { message = "註冊失敗，該 Email 已被使用" });
            }

            return Ok(new { message = "註冊成功" });
               
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new { message = "帳號密碼格式錯誤" });
            }

            var token = await _userService.LoginAsync(model);

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

            if (!result.success) return BadRequest(new { message = result.message }); // 使用 Service 傳回的訊息

            return Ok(new { message = "驗證成功，帳號已啟用，請重新登入" });
        }

        [HttpPost("ResendOtp")]
        public async Task<IActionResult> ResendOtp([FromBody] UserResendOtpViewModel model)
        {
            if (string.IsNullOrEmpty(model.Email))
                return BadRequest(new { message = "Email 不能為空" });

            var result = await _userService.ResendOtpAsync(model.Email);

            if (!result)
                return BadRequest(new { message = "找不到該帳號或帳號已啟用" });

            return Ok(new { message = "新的驗證碼已寄出" });
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
        public async Task<IActionResult> ForgotPassword([FromBody] UserForgotPasswordViewModel model)
        {

            if (string.IsNullOrEmpty(model.Email))
                return BadRequest(new { message = "請輸入 Email" });

            var result = await _userService.SendPasswordResetOtpAsync(model.Email);

            // 專案演示用：回傳具體的錯誤
            if (!result) return BadRequest(new { message = "找不到該帳號或發送失敗" });

            return Ok(new { message = "驗證碼已寄出" });
        }

        [HttpPost("VerifyPasswordResetOtp")]
        public async Task<IActionResult> VerifyResetOtp([FromBody] UserOtpVerifyViewModel model)
        {
            var (success, message) = await _userService.VerifyPasswordResetOtpAsync(model.Email, model.Otp);
            if (!success) return BadRequest(new { message });
            return Ok(new { message });
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] UserResetPasswordViewModel model)
        {
            var success = await _userService.ResetPasswordAsync(model.Email, model.Otp, model.NewPassword);
            if (!success) return BadRequest(new { message = "驗證碼錯誤或已過期" });
            return Ok(new { message = "密碼重設成功" });
        }


        [HttpPost("AskXiaoSha")]
        public async Task<IActionResult> AskXiaoSha([FromBody] ChatRequest request)
        {
            Console.WriteLine("==== 小沙接收到請求了！ ====");
            if (string.IsNullOrEmpty(request.Message))
                return BadRequest(new { message = "旅伴，你想問小沙什麼呢？🌊" });
            Console.WriteLine($"收到訊息內容: {request.Message}");

            try
            {
                using var httpClient = new HttpClient();

                // 1. 組合 Prompt (完全照抄你的 Python 邏輯)
                var fullPrompt = $"{_systemInstruction}\n\n現在有一位旅伴問了這個問題：{request.Message}";

                // 2. 準備 Data (完全照抄你的 Python 結構)
                var data = new
                {
                    contents = new[]
                    {
                new { parts = new[] { new { text = fullPrompt } } }
            }
                };

                // 3. 設定 URL (完全照抄你的 Python URL: v1beta + gemini-flash-latest)
                var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent?key={_apiKey}";

                // 4. 發送 POST 請求
                var response = await httpClient.PostAsJsonAsync(url, data);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<JsonElement>();

                    // 5. 提取回覆 (完全照抄你的 Python 提取路徑)
                    string botReply = result.GetProperty("candidates")[0]
                                            .GetProperty("content")
                                            .GetProperty("parts")[0]
                                            .GetProperty("text")
                                            .GetString();

                    return Ok(new { reply = botReply });
                }
                else
                {
                    var errorResult = await response.Content.ReadAsStringAsync();
                    return BadRequest(new { error = $"Gemini API 錯誤: {errorResult}" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"連線發生意外：{ex.Message}" });
            }
        }

    }

    public class ChatRequest 
    { 
        public string Message { get; set; } = string.Empty; 
    }


    
}
