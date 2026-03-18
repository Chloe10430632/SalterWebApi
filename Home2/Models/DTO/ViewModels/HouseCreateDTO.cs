using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HomeServiceHelper.Models.DTO.ViewModels
{
    public class HouseCreateDTO
    {
        // 對應 HomHouse 表
        public int UserID { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Citie { get; set; } = string.Empty;

        // 對應 HomRoomType 表
        public string RoomName { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public int Capacity { get; set; }

        // 圖片部分：改用 string 列表接收 Cloudinary 網址
        public List<string> ImageUrls { get; set; } = new();

        // 設備部分：接收前端傳來的 ID 陣列 (例如 [1, 2, 5])
        public List<int> AmenityIds { get; set; } = new();
    }

}
