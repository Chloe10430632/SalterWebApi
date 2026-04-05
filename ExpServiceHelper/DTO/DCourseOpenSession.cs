using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpServiceHelper.DTO
{
    public class DCourseOpenSession
    {
        public int? SessionId { get; set; }

        public string? TimeSlot { get; set; } = string.Empty;// 譬如 "09:00-11:00"
        public int? MaxParticipants { get; set; }
        public DateOnly? StartDate {  get; set; }

        internal IEnumerable<object> Parse(object startDate)
        {
            throw new NotImplementedException();
        }
        // public DateOnly? EndDate { get; set; }

    }
}
