using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServiceHelper.Models.DTO.ViewModel
{
    public class UserEditViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Gender { get; set; }
        public DateOnly? Birthday { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
