using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpServiceHelper.DTO
{
    public class DCourseOpenSession
    {
        public int TemplateId { get; set; }
        public string TimeSlot { get; set; } // 譬如 "09:00-11:00"
        public int MaxStudents { get; set; }
        public List<DateTime> SelectedDates { get; set; } = new List<DateTime>(); // 這裡就是放 [4/13, 4/27]
    }
}
