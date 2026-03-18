using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpServiceHelper.DTO
{
    public class DCourseOpenSession
    {
        public int TemplateId { get; set; }  //追蹤用
        public string TimeSlot { get; set; } = string.Empty;// 譬如 "09:00-11:00"
        public int MaxStudents { get; set; }
        public List<DateTime> SelectedDates { get; set; } = new List<DateTime>();
        public int CoachId { get; set; }//誰開課
    }
}
