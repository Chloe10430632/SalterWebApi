using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServiceHelper.Models.DTO.ViewModel
{
    public class UserProfileViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Gender { get; set; }
        public DateOnly? Birthday { get; set; }
        public string? ProfilePicture { get; set; }
        public string RoleName { get; set; } = null!;

    }
}
