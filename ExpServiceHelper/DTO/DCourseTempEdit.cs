using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpServiceHelper.DTO
{
    public class DCourseTempEdit
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Difficulty { get; set; }
        public decimal? Price { get; set; }
        public string? Location { get; set; }
        public List<IFormFile> PhotoUrls { get; set; } = new List<IFormFile>();
        public string? PublicId { get; set; }

    }
}
