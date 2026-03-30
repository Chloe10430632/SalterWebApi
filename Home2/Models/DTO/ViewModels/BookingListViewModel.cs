using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServiceHelper.Models.DTO.ViewModels
{
    public class BookingListViewModel
    {
        public int BookingId { get; set; }        // 訂單編號
        public string? RoomTypeName { get; set; } = string.Empty; // 房間名稱
        public string? RoomImage { get; set; }    
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "0"; // 狀態 (0:待付款, 1:已付款, 2:已完成, 3:處理中, 4:已取消)
        public DateTime CreatedTime { get; set; }  // 下單時間
    }
}
