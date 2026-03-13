using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServiceHelper.Models.DTO.ViewModels
{
    public class HouseSearchDTO
    {
        public string? Citie { get; set; }// 城市
        public int? PeopleCount { get; set; }// 人數
        public DateTime? StartDate { get; set; } // 開始日期
        public DateTime? EndDate { get; set; } // 結束日期
    }
}
