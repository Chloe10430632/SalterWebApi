using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpServiceHelper.DTO
{
    public class DCoachFavList
    {
        public int UserId { get; set; }
        public int CoachId { get; set; }

        //===收藏清單===//
        public string CoachName { get; set; } = null!;
        public List<string?> District { get; set; } = new List<string>();
        public string? City { get; set; }
        public double AvgRating { get; set; }
        public int ReviewCount { get; set; }
        public List<string> Specialities { get; set; } = new List<string>();
        public IFormFile? AvatarFile { get; set; }
        public string? PublicId { get; set; }

    }
}
