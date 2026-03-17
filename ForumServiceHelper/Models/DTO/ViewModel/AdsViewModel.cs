using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumServiceHelper.Models.DTO.ViewModel
{
    public class AdsViewModel
    {
        public int Id { get; set; }

        public string TooltipText { get; set; } = string.Empty;

        public string TargetUrl { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;
    }
}
