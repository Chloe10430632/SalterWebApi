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

namespace UserServiceHelper.Service
{
    public class UserService : IUserService
    {
        private readonly IGenericUserRepository<UserUser> _dbUser;
        private readonly PasswordHasher<UserUser> _passwordHasher;
        private readonly IConfiguration _configuration;

        public UserService(IGenericUserRepository<UserUser> dbUser, PasswordHasher<UserUser> passwordHasher, IConfiguration configuration)
        {
            _dbUser = dbUser;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
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

        public async Task<bool> UpdateProfileAsync(UserEditViewModel model)
        {
            var userInDb = await _dbUser.GetByIdAsync(model.Id);
            if (userInDb == null)
                return false;

            userInDb.UserName = model.UserName;
            userInDb.Phone = model.Phone;
            userInDb.Gender = model.Gender;
            userInDb.Birthday = model.Birthday;
            userInDb.ProfilePicture = model.ProfilePicture;
            userInDb.UpdatedAt = DateTime.Now;

            _dbUser.Update(userInDb);

            return await _dbUser.SaveChangesAsync();

        }

        public async Task<bool> RegisterAsync(UserRegisterViewModel Rmodel)
        {
            var dbContext = _dbUser.GetDbContext();
            if (await dbContext.UserUsers.AnyAsync(u => u.Email == Rmodel.Email))
                return false;

            var newUser = new UserUser
            {
                UserName = Rmodel.UserName,
                Email = Rmodel.Email,
                Phone = Rmodel.Phone,
                Gender = Rmodel.Gender,
                Birthday = Rmodel.Birthday,              
                ProfilePicture = Rmodel.ProfilePicture ?? "/admin/imgs/default-avatar.png",

                UserRoleId = 1,
                StatusId = 1,
                IsActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now

                

            };

            
            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, Rmodel.Password);

            await _dbUser.CreateAsync(newUser);

            return await _dbUser.SaveChangesAsync();
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


            //驗證成功，產生真正的 JWT 字串
            return GenerateJwtToken(User);

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
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.Id.ToString()),
                new Claim("UserName", user.UserName),
                // 把你的圖片路徑塞進去
                new Claim("Avatar", user.ProfilePicture ?? "/admin/imgs/default-avatar.png"),
                // 塞入角色名稱
                new Claim(ClaimTypes.Role, user.UserRole?.Name ?? "一般會員")
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

    }
}
