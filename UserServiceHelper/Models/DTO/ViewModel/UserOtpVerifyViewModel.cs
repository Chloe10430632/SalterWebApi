using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServiceHelper.Models.DTO.ViewModel
{
    public class UserOtpVerifyViewModel
    {
        public string Email { get; set; } = null!;
        public string Otp { get; set; } = null!;
    }
}
