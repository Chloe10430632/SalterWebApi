using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SalterEFModels.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public UserService(IGenericUserRepository<UserUser> dbUser, PasswordHasher<UserUser> passwordHasher)
        {
            _dbUser = dbUser;
            _passwordHasher = passwordHasher;
        }
        
        public async Task<UserProfileViewModel?> GetUserProfileAsync(int userId)
        {
            var user = await _dbUser.GetByIdAsync(userId);

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
                RoleName = "一般會員"
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

            var User = await dbContext.UserUsers.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (User == null)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(User, User.PasswordHash, model.Password);

            if (result == PasswordVerificationResult.Failed)
                return null;

            return "LoginSuccess_Token_Placeholder";

        }
    }
}
