using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpServiceHelper.DTO
{
    public class DCourseInfo
    {
        public int? TempId { get; set; }
        public int? SessionId { get; set; } 
        public int? CoachId { get; set; }
        public string? TimeSlot { get; set; } = string.Empty;// 譬如 "09:00-11:00"
        public int? MaxParticipants { get; set; }
        public int? CurrentParticipants { get; set; }
        public DateTime? UpdatedAt { get; set; }
        //
        public List<IFormFile>? PhotoUrls { get; set; } = new List<IFormFile>();
        public string? PublicId { get; set; }
        public List<DPhoto> ImageUrls { get; set; }
        //
        public string? Title { get; set; }
        public string? Difficulty { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? GooglePlaceId {  get; set; }
        //
        public DateOnly? StartDate { get; set; }
        // public DateOnly? EndDate { get; set; }
        public int? ReviewId { get; set; }
        public int? CoachUserId { get; set; }
    }
}
