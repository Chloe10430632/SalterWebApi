using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HomeServiceHelper.Models.DTO.ViewModels
{
    public class HouseCreateDTO
    {
        // 房屋基本資訊 (HomHouse)
        public int UserID { get; set; } = 1; // 預設一個房東ID
        public string? HouseDescription { get; set; }
        public string? Location { get; set; }
        public string? District { get; set; }
        public string? Citie { get; set; }

        // 房型資訊 (HomRoomType)
        public string? RoomName { get; set; }
        public int Capacity { get; set; }
        public decimal PricePerNight { get; set; }
        public string? RoomDescription { get; set; }

        // 圖片網址列表 (準備存入 HomRoomImage)
        public List<string> ImageUrls { get; set; } = new List<string>();
    }

}
