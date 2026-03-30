using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SalterEFModels.EFModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserRepositoryHelper.IRepository;
using UserServiceHelper.IService;
using UserServiceHelper.Models.DTO.ViewModel;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using ExpServiceHelper.IService;

namespace UserServiceHelper.Service
{
    public class UserService : IUserService
    {
        private readonly IGenericUserRepository<UserUser> _dbUser;
        private readonly PasswordHasher<UserUser> _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly ISCoachMethods _coachMethods;


        public UserService(IGenericUserRepository<UserUser> dbUser, PasswordHasher<UserUser> passwordHasher, IConfiguration configuration, ISCoachMethods sCoachMethods)
        {
            _dbUser = dbUser;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _coachMethods = sCoachMethods;
        }
        
        public async Task<UserProfileViewModel?> GetUserProfileAsync(int userId)
        {
            var user = await _dbUser.GetDbContext().UserUsers
                        .Include(u => u.UserRole)
                        .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return null;

            return new UserProfileViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.Phone,
                Gender = user.Gender,
                Birthday = user.Birthday,
                ProfilePicture = user.ProfilePicture,
                RoleName = user.UserRole?.Name ?? "一般會員"
            };

        }

        public async Task<string?> UpdateProfileAsync(UserEditViewModel model)
        {
            var userInDb = await _dbUser.GetByIdAsync(model.Id);
            if (userInDb == null)
                return null;

            userInDb.UserName = !string.IsNullOrWhiteSpace(model.UserName) ? model.UserName : userInDb.UserName;
            userInDb.Phone = model.Phone;
            userInDb.Gender = model.Gender;
            userInDb.Birthday = model.Birthday;
            userInDb.ProfilePicture = !string.IsNullOrWhiteSpace(model.ProfilePicture) ? model.ProfilePicture : userInDb.ProfilePicture;
            userInDb.UpdatedAt = DateTime.Now;

            _dbUser.Update(userInDb);
            var success = await _dbUser.SaveChangesAsync();

            return success ? GenerateJwtToken(userInDb) : null;

        }

        public async Task<bool> RegisterAsync(UserRegisterViewModel model)
        {
            var dbContext = _dbUser.GetDbContext();
            if (await dbContext.UserUsers.AnyAsync(u => u.Email == model.Email))
                return false;

            string otp = new Random().Next(100000, 999999).ToString();

            var newUser = new UserUser
            {
                UserName = model.UserName,
                Email = model.Email,
                Phone = model.Phone,
                Gender = model.Gender,
                Birthday = model.Birthday,              
                ProfilePicture = string.IsNullOrWhiteSpace(model.ProfilePicture)
                 ? "/admin/imgs/default-avatar.png"
                 : model.ProfilePicture,

                UserRoleId = 1,
                StatusId = 1,
                IsActive = false,

                EmailVerificationOtp = otp,
                EmailVerificationExpiresAt = DateTime.Now.AddMinutes(5),

                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now

            };

            
            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, model.Password);

            await _dbUser.CreateAsync(newUser);

            var saved = await _dbUser.SaveChangesAsync();

            if (saved)
            {
                // 背景執行，不 await，直接讓 API 回傳成功給前端
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await SendEmailInternalAsync(newUser.Email, newUser.UserName, otp);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"註冊信寄送失敗：{ex.Message}");
                    }
                });
            }

            return saved;
        }

        public async Task<string?> LoginAsync(UserLoginViewModel model)
        {
            var dbContext =  _dbUser.GetDbContext();

            var User = await dbContext.UserUsers.Include(u => u.UserRole)
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (User == null)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(User, User.PasswordHash, model.Password);

            if (result == PasswordVerificationResult.Failed)
                return null;



            if (!User.IsActive) return "WAITING_FOR_EMAIL_VERIFICATION";

            //驗證成功，產生真正的 JWT 字串
            return GenerateJwtToken(User);

        }

        public async Task<(bool success, string message)> VerifyRegistrationOtpAsync(string email, string otp)
        {
            var dbContext = _dbUser.GetDbContext();

            var user = await dbContext.UserUsers.FirstOrDefaultAsync(u => u.Email == email);

            // 如果找不到，代表驗證碼錯了、或是過期了
            if (user == null) return (false, "找不到使用者");

            if (user.IsActive) return (true, "帳號已啟用，請直接登入");

            if (user.EmailVerificationOtp != otp)  return (false, "驗證碼錯誤");

            if (user.EmailVerificationExpiresAt < DateTime.Now) return (false, "驗證碼已過期，請重新取得");



            // 驗證成功：
            user.IsActive = true;               // 啟用帳號
            user.EmailVerifiedAt = DateTime.Now; // 紀錄驗證時間

            // 清空驗證碼欄位（用過即作廢，這是資安好習慣）
            user.EmailVerificationOtp = null;
            user.EmailVerificationExpiresAt = null;

            _dbUser.Update(user);
            var saved = await _dbUser.SaveChangesAsync();

            if (saved)
                return (true, "驗證成功！");
            else
                return (false, "資料庫更新失敗");

        }

        public async Task<bool> ResendOtpAsync(string email)
        {
            var user = await _dbUser.GetDbContext().UserUsers
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsActive);

            if (user == null) return false;

            // 1. 產生新的 6 位數
            string newOtp = new Random().Next(100000, 999999).ToString();

            // 2. 更新資料庫
            user.EmailVerificationOtp = newOtp;
            user.EmailVerificationExpiresAt = DateTime.Now.AddMinutes(5);
            _dbUser.Update(user);
            var saved = await _dbUser.SaveChangesAsync();

            // 3. 再次寄信 (沿用你剛才寫好的邏輯)
            if (saved)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await SendEmailInternalAsync(user.Email, user.UserName, newOtp);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"重發驗證信失敗：{ex.Message}");
                    }
                });
            }

            return saved;
        }

        public async Task<string?> GoogleLoginAsync(string idToken)
        {
            try
            {
                // 驗證設定：確保這張票是發給我們專案的 (ClientId)
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string> { _configuration["Google:ClientId"]! }
                };

                // 核心驗證：這行會幫你擋掉變造、過期的 Token
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                // 建議加上：確保 Google 帳號本身是經過 Email 驗證的
                if (!payload.EmailVerified) return null;

                var dbContext = _dbUser.GetDbContext();
                var user = await dbContext.UserUsers
                    .Include(u => u.UserRole)
                    .FirstOrDefaultAsync(u => u.Email == payload.Email);

                // 如果資料庫沒這個人，就地註冊
                if (user == null)
                {
                    user = new UserUser
                    {
                        UserName = payload.Name ?? payload.Email.Split('@')[0],
                        Email = payload.Email,
                        ProfilePicture = payload.Picture ?? "/admin/imgs/default-avatar.png",
                        UserRoleId = 1, // 預設會員
                        StatusId = 1,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        PasswordHash = _passwordHasher.HashPassword(user, Guid.NewGuid().ToString())
                    };
                    await _dbUser.CreateAsync(user);
                    await _dbUser.SaveChangesAsync();

                    // 重新載入以包含 UserRole 資訊
                    user = await dbContext.UserUsers.Include(u => u.UserRole).FirstOrDefaultAsync(u => u.Id == user.Id);
                }

                return GenerateJwtToken(user!);
            }
            catch (Exception ex)
            {
                Console.WriteLine("發生錯誤了！原因：" + ex.Message);
                if (ex.InnerException != null)
                    Console.WriteLine("內部詳細錯誤：" + ex.InnerException.Message);
                return null;
            }
        }

        public async Task<bool> SendPasswordResetOtpAsync(string email)
        {
            var user = await _dbUser.GetDbContext().UserUsers
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null) return false;

            // 產生 6 位數 OTP
            string otp = new Random().Next(100000, 999999).ToString();

            // 存入你資料庫原本就有的忘記密碼欄位
            user.PasswordResetOtp = otp;
            user.PasswordResetExpiresAt = DateTime.Now.AddMinutes(10); // 設 10 分鐘過期

            _dbUser.Update(user);
            var saved = await _dbUser.SaveChangesAsync();

            if (saved)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        // 這裡的主旨可能要稍微區分，或者你在 SendEmailInternalAsync 裡加個參數
                        await SendEmailInternalAsync(user.Email, user.UserName, otp);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"忘記密碼信寄送失敗：{ex.Message}");
                    }
                });
            }

            return saved;
        }

        public async Task<(bool success, string message)> VerifyPasswordResetOtpAsync(string email, string otp)
        {
            var user = await _dbUser.GetDbContext().UserUsers
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null) return (false, "找不到使用者");
            if (user.PasswordResetOtp != otp) return (false, "驗證碼錯誤");
            if (user.PasswordResetExpiresAt < DateTime.Now) return (false, "驗證碼已過期");

            return (true, "驗證成功");
        }


        public async Task<bool> ResetPasswordAsync(string email, string otp, string newPassword)
        {
            var user = await _dbUser.GetDbContext().UserUsers
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || user.PasswordResetOtp != otp || user.PasswordResetExpiresAt < DateTime.Now)
                return false;

            // 驗證成功，雜湊新密碼
            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            user.IsActive = true;
            user.EmailVerifiedAt = DateTime.Now;
            // 清空 OTP 確保安全性
            user.PasswordResetOtp = null;
            user.PasswordResetExpiresAt = null;

            user.PasswordChangedAt = DateTime.Now;
            _dbUser.Update(user);
            return await _dbUser.SaveChangesAsync();
        }



        // --- 核心工具：產生 JWT 憑證 ---
        private string GenerateJwtToken(UserUser user)
        {
            // 1. 從 appsettings.json 讀取密鑰
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 2. 準備「通行證」上面的資訊 (Claims)
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("UserName", user.UserName),
                new Claim(ClaimTypes.Email,user.Email),
                // 把你的圖片路徑塞進去
                new Claim("Avatar", user.ProfilePicture ?? "/admin/imgs/default-avatar.png"),
                // 塞入角色名稱
                new Claim(ClaimTypes.Role, user.UserRole?.Name ?? "一般會員"),
               new Claim("CoachId", user.ExpCoaches.FirstOrDefault()?.Id.ToString() ?? "0")
            };


            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: creds
            );

            // 4. 轉換成字串回傳
            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        //MailKit 寄信代碼
        private async Task SendEmailInternalAsync(string toEmail, string userName, string otp)
        {
            var message = new MimeMessage();
            // 建議明確指定名稱與地址，避免 null 導致的錯誤
            message.From.Add(new MailboxAddress("Salter 團隊", _configuration["EmailSettings:SmtpUser"]));
            message.To.Add(new MailboxAddress(userName, toEmail));
            message.Subject = "Salter 專案 - 您的帳號驗證碼";
            string htmlBody = $@"
    <div style='background-color: #f8fafc; padding: 50px 20px; font-family: ""Microsoft JhengHei"", Helvetica, Arial, sans-serif;'>
        <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 24px; overflow: hidden; box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1); border: 1px solid #e2e8f0;'>

            <div style='background-color: #8B5A2B; height: 8px;'></div>

            <div style='padding: 40px;'>
                <div style='text-align: center; margin-bottom: 30px;'>
                    <img src='https://res.cloudinary.com/dc65nejyp/image/upload/v1774510422/nkj7q-lehyh_vkffmq.png' 
                         alt='Salter Logo' 
                         style='width: 200px; height: auto; display: block; margin: 0 auto; border: 0;'>
                    <p style='color: #94a3b8; font-size: 12px; margin-top: 10px; text-transform: uppercase; letter-spacing: 2px;'>
                        Ocean Adventure & Lifestyle
                    </p>
                </div>

                <h2 style='color: #1e293b; font-size: 20px; margin-bottom: 20px; text-align: center;'>驗證您的電子信箱</h2>

                <p style='color: #475569; line-height: 1.6; font-size: 16px;'>
                    {userName} 您好，<br>
                    感謝您加入 Salter！請使用下方的驗證碼完成您的帳號操作：
                </p>

                <div style='background-color: #f1f5f9; border-radius: 16px; padding: 30px; margin: 30px 0; text-align: center;'>
    <span style='display: block; color: #64748b; font-size: 13px; margin-bottom: 10px; font-weight: bold; letter-spacing: 1px;'>您的驗證碼如下</span>
    
    <span style='color: #8B5A2B; 
                 font-size: 36px; 
                 font-weight: 900; 
                 letter-spacing: 6px; 
                 font-family: monospace; 
                 white-space: nowrap; 
                 display: inline-block;'>
        {otp}
    </span>
</div>

                <p style='color: #ef4444; font-size: 14px; text-align: center; font-weight: bold;'>
                    ⚠️ 請在 5 分鐘內輸入。過期後需重新申請。
                </p>

                <div style='border-top: 1px solid #e2e8f0; margin-top: 40px; padding-top: 20px; text-align: center;'>
                    <p style='color: #94a3b8; font-size: 13px; line-height: 1.5;'>
                        如果您沒有嘗試登入或註冊 Salter，請忽略此郵件。<br>
                        這是一封系統自動發送的信件，請勿直接回覆。
                    </p>
                </div>
            </div>

            <div style='background-color: #f8fafc; padding: 20px; text-align: center; border-top: 1px solid #e2e8f0;'>
                <p style='color: #cbd5e1; font-size: 12px; margin: 0;'>© 2026 Salter Project. All Rights Reserved.</p>
            </div>
        </div>
    </div>";

            //message.Body = new TextPart("html")
            //{
            //    Text = $@"<div style='border:1px solid #ddd; padding:20px; max-width:500px;'>
            //        <h2 style='color:#2c3e50;'>Salter 驗證碼服務</h2>
            //        <p>{userName} 您好，您的驗證碼如下：</p>
            //        <h1 style='color:#3498db; text-align:center;'>{otp}</h1>
            //        <p style='color:#e74c3c;'>請在 5 分鐘內輸入。如果不是您本人操作，請忽略此信。</p>
            //     </div>"
            //};

            message.Body = new TextPart("html") { Text = htmlBody };


            // 連接 Gmail SMTP
            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_configuration["EmailSettings:SmtpUser"], _configuration["EmailSettings:AppPassword"]);
                await client.SendAsync(message);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }





    }
}
