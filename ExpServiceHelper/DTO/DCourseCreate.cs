using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpServiceHelper.DTO
{
    public class DCourseCreate
    {
        public int? CoachId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int Difficulty { get; set; }
        public decimal? Price { get; set; }
        public string? Location { get; set; }
        public string? PhotoUrl { get; set; }

    }
}
