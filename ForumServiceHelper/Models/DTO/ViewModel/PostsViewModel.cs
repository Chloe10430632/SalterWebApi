using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumServiceHelper.Models.DTO.ViewModel
{
    public class PostsViewModel
    {
        public int PostId { get; set; }

        // 發文者
        public string UserName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string BoardTitle { get; set; } = "預設";

        // 內容
        public string Content { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new();
        public int AgoMinuteNumber { get; set; }
        public int AgoHourNumber { get; set; }
        public int AgoDayNumber { get; set; }

        // 統計數據
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public int BookmarkCount { get; set; }
        public int ShareCount { get; set; }

        // 留言預覽 (通常只取最新或熱門的一兩則)
        public List<CommentPreviewDto> LatestComments { get; set; } = new();
    }
    public class CommentPreviewDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
