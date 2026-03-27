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
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int MemberId { get; set; } // 對應 UserId
        public int BookingId { get; set; } // 連結訂單與評論
    }
}
