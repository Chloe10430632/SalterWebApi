using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpServiceHelper.DTO
{
    public class DCoachReview
    {
        public int CoachId { get; set; }
        public int? Rating { get; set; }
        public string? ReviewContent { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public int? CourseOrderId { get; set; }
    }
}
