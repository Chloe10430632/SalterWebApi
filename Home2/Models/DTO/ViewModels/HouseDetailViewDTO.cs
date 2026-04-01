using HomeServiceHelper.Models.DTO.ViewModels.Review;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServiceHelper.Models.DTO.ViewModels
{
    public class HouseDetailViewDTO
    {
        
        public string? Name { get; set; }
        public int RoomTypeId { get; set; }//點擊預約可能會用到
        public int? Capacity { get; set; }// 人數
        public decimal? PricePerNight { get; set; }
        public int? ViewCount { get; set; }// 瀏覽次數
        public string? RoomDescription { get; set; }// 房型描述
        public string? HouseDescription { get; set; }

        public string? Location { get; set; }// 位置
        public string? District { get; set; }// 區域
        public string? Citie { get; set; }// 城市

        public List<string> AllImages { get; set; } = new(); // 所有房型圖片
        public List<AmenityItemDTO> Amenities { get; set; } = new(); // 設施清單
        public List<int>? AmenityIds { get; set; }
        public List<ReviewItemDTO> Reviews { get; set; } = new();
        public bool IsAlreadyBooked { get; set; } //是否已預約（待付款或已付款）

    }
}
