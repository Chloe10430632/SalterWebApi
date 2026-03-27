using SalterEFModels.EFModels;
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

        Task<string?> UpdateProfileAsync(UserEditViewModel model);

        Task<bool> RegisterAsync(UserRegisterViewModel model);

        Task<string?> LoginAsync(UserLoginViewModel model);

        Task<string?> GoogleLoginAsync(string idToken);

        Task<(bool success, string message)> VerifyRegistrationOtpAsync(string email, string otp);

        Task<bool> ResendOtpAsync(string email);

        Task<bool> SendPasswordResetOtpAsync(string email);

        Task<(bool success, string message)> VerifyPasswordResetOtpAsync(string email, string otp);

        Task<bool> ResetPasswordAsync(string email, string otp, string newPassword);



    }
}
