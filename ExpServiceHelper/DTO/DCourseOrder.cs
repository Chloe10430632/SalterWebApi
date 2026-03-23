using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpServiceHelper.DTO
{
    public class DCourseOrder
    {
        public int? CourseSessionId { get; set; }
        public DateTime? ReservedAt { get; set; }
        public int? ExpTransactionId { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? Status { get; set; }
    }
}
