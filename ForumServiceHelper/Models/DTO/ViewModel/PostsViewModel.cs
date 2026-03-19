using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumServiceHelper.Models.DTO.ViewModel
{
    // 列表專用：拿掉 Comments，讓前端在首頁加載飛快
    public class PostListViewModel
    {
        public int PostId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string BoardTitle { get; set; } = "預設";
        public string LocationTitle { get; set; } = "預設";
        public string ContentPreview { get; set; } = string.Empty; // 內容摘要
        public List<string> ImageUrls { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public int LikeCount { get; set; }
        public int CollectCount { get; set; }
        public int ShareCount { get; set; }
        public int CommentCount { get; set; }
        public int ViewCount { get; set; }
        public List<string> PostTags { get; set; } = new();
    }

    // 詳情專用：繼承列表並擴充留言與詳細數據
    public class PostDetailViewModel : PostListViewModel
    {
        public int BookmarkCount { get; set; }
        public int ShareCount { get; set; }
        public string FullContent { get; set; } = string.Empty;
        public List<CommentPreviewDto> Comments { get; set; } = new();
    }
    public class CommentPreviewDto
    {
        public int CommentId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        //用來存放該留言底下的子回覆
        public List<CommentPreviewDto> Replies { get; set; } = new();
    }
}
