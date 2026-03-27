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
        public int? CoachId { get; set; }
        public List<DateOnly>? SelectedDates { get; set; } = new List<DateOnly>();
        public string? TimeSlot { get; set; } = string.Empty;// 譬如 "09:00-11:00"
        public int? MaxStudents { get; set; }
        public int? CurrentStudents { get; set; }
        public DateTime? UpdatedAt { get; set; }
        //
        public List<IFormFile>? PhotoUrls { get; set; } = new List<IFormFile>();
        public string? PublicId { get; set; }

    }
}
