using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumServiceHelper.Models.DTO.CreateModel
{
    public  class PostInteractionCreateModel
    {
        public int PostId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? ReportReason { get; set; } //檢舉原因
    }
}
