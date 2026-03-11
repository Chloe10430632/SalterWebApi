using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServiceHelper.Models.DTO.ViewModel
{
    public class UserLoginViewModel
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;

    }
}
