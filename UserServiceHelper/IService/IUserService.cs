using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceHelper.Models.DTO.ViewModel;

namespace UserServiceHelper.IService
{
    public interface IUserService
    {
        Task<UserProfileViewModel?> GetUserProfileAsync(int userId);

        Task<bool> UpdateProfileAsync(UserEditViewModel model);

        Task<bool> RegisterAsync(UserRegisterViewModel Rmodel);

        Task<string?> LoginAsync(UserLoginViewModel model);

        Task<string?> GoogleLoginAsync(string idToken);

    }
}
