using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServiceHelper.Models.DTO.ViewModels
{
    public class HouseUpdateDTO
    {
        public int HouseId { get; set; }
        public int RoomTypeId { get; set; }
        // 其他欄位跟 CreateDTO 一樣
        public string Citie { get; set; }
        public string District { get; set; }
        public string Location { get; set; }
        public string HouseDescription { get; set; }
        public string RoomName { get; set; }
        public decimal PricePerNight { get; set; }
        public int Capacity { get; set; }
        public string RoomDescription { get; set; }
        public List<string> ImageUrls { get; set; }
        public List<int> AmenityIds { get; set; }
    }
}
