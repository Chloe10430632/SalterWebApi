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
        
        public UserService(IGenericUserRepository<UserUser> dbUser)
        {
            _dbUser = dbUser;
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

            userInDb.Phone = model.Phone;
            userInDb.Gender = model.Gender;
            userInDb.Birthday = model.Birthday;
            userInDb.ProfilePicture = model.ProfilePicture;
            userInDb.UpdatedAt = DateTime.Now;

            _dbUser.Update(userInDb);

            return await _dbUser.SaveChangesAsync();

        }
    }
}
