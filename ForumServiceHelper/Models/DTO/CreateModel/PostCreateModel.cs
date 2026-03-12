using Microsoft.AspNetCore.Http;
using SalterEFModels.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumServiceHelper.Models.DTO.CreateModel
{
    public class PostCreateModel
    {
        public int UserId { get; set; }

        public int BoardId { get; set; }

        public string Content { get; set; } = string.Empty;

        public int? LocationId { get; set; }

        public string Status { get; set; } = "NORMAL";  //顯示或隱藏

        // 圖片：前端傳入多張圖片的 URL
        public List<IFormFile>? Images { get; set; }

        // 標籤：前端傳入標籤 ID
        public List<TagCreateModel>? Tags { get; set; }

    }

    public class TagCreateModel
    {
        public int TagId { get; set; }
        public string TagName { get; set; } = null!;
    }
}
