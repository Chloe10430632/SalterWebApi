using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServiceHelper.Models.DTO.ViewModels
{
    public class HousePreviewDTO
    {
        public string? RoomTypeName { get; set; }
        public int HouseId { get; set; }
        public int RoomTypeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string District { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}
