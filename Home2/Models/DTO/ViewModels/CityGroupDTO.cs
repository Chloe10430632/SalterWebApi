using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServiceHelper.Models.DTO.ViewModels
{
    public class CityGroupDTO
    {
        public string CityName { get; set; } = string.Empty;
        public List<HousePreviewDTO> Houses { get; set; } = new();
    }
}
