using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpServiceHelper.DTO
{
    public class DCoachRecommend
    {
        public int CoachId { get; set; }
        public string CoachName { get; set; } = null!;
        public string? AvatarUrl { get; set; }
        public string? City { get; set; }
        public List<string> Specialities { get; set; } = new List<string>();//
    }
}
