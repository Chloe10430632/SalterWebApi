using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpServiceHelper.DTO
{
    public class DCoachInfo
    {
        public int CoachId { get; set; }
        public string CoachName { get; set; } = null!;
        public string? AvatarUrl { get; set; }
        public string? District { get; set; }
        public DateTime? CreatedAt { get; set; }
        public double AvgRating { get; set; }
        public int ReviewCount { get; set; }
        public List<string> Specialities { get; set; } = new List<string>();
    }
}
