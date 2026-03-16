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

        public bool isPosted { get; set; }

        // 圖片：上傳完圖床後，前端傳入多張圖片的 URL
        public List<string>? ImageUrls { get; set; } = new();

        // 標籤：前端傳入標籤 ID
        public List<TagCreateModel>? Tags { get; set; } = new();

    }

    public class TagCreateModel
    {
        public int TagId { get; set; }
        public string TagName { get; set; } = null!;
    }
}
