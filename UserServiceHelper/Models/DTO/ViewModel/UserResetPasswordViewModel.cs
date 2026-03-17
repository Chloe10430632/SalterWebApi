using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServiceHelper.Models.DTO.ViewModel
{
    public class UserResetPasswordViewModel
    {
        public string Email { get; set; } = string.Empty;

        // 使用者從信箱收到的 6 位數 OTP
        public string Otp { get; set; } = string.Empty;

        // 使用者想要設定的新密碼
        public string NewPassword { get; set; } = string.Empty;
    }
}
