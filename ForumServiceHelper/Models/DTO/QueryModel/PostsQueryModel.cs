using ForumServiceHelper.Models.DTO.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumServiceHelper.Models.DTO.QueryModel
{
    public class PostsQueryModel
    {
        public string SortBy { get; set; } = SortTypes.Popular; // NEW, POPULAR, FOLLOW
        public string? Keyword { get; set; }
        public int? BoardId { get; set; }
        public int? UserId { get; set; } // 用於 Follow 排序

        // 游標分頁參數
        public int? LastId { get; set; }
        public DateTime? LastCreatedAt { get; set; }
        public int? LastViewCount { get; set; }
        public int TakeSize { get; set; } = 5;
    }
}
