using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServiceHelper.Models.DTO.ViewModels
{
    public class ReviewItemDTO
    {
        public int? Rating { get; set; } //
        public string? Comment { get; set; } //
        public DateTime? CreatedTime { get; set; } //
        public string? Name { get; set; }
        public string? Picture { get; set; }
    }
}
