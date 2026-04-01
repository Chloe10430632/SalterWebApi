using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServiceHelper.Models.DTO.ViewModels.Review
{
    public class ReviewUpdateDTO
    {
        public int ReviewId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int MemberId { get; set; } // 對應UserId
    }
}
