using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServiceHelper.Models.DTO.ViewModels
{
    public class ReviewCreateDTO
    {
        public int RoomTypeId { get; set; }
        public int Rating { get; set; } // 1-5 星
        public string Comment { get; set; } = string.Empty;
        public int MemberId { get; set; } // 先假定一個會員 ID
    }
}
