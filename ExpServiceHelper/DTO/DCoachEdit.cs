using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpServiceHelper.DTO
{
    public class DCoachEdit
    {
        //只放讓user異動的欄位
       
        public string? Name { get; set; } 
        public string? AvatarUrl { get; set; }
        public string? Introduction { get; set; }
        public int? DistrictId { get; set; }
        public int? CityId { get; set; }
        public string? CityName { get; set; } 
        public string? DistrictName { get; set; } 
    }
}
