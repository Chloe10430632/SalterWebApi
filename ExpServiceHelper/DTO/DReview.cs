using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpServiceHelper.DTO
{
    public class DReview
    {
        public int? ReviewId { get; set; }
        public int? UserId { get; set; }
        public int? CoachId { get; set; }
        public int? Rating { get; set; }
        public string? ReviewContent { get; set; }
        public int? CourseOrderId { get; set; }
        public DateTime? UpdateAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public int status { get; set; }
        public string? UserName { get; set; }

    }
}
