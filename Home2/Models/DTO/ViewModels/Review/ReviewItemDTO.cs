using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServiceHelper.Models.DTO.ViewModels.Review
{
    public class ReviewItemDTO
    {
        public int ReviewId { get; set; }
        public int UserId { get; set; }
        public int? Rating { get; set; } 
        public string? Comment { get; set; } 
        public DateTime? CreatedTime { get; set; } 
        public string? Name { get; set; }
        public string? Picture { get; set; }
    }
}
