using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumServiceHelper.Models.DTO.Const
{
    // 定義貼文互動類型的常數，對應資料庫中的 PostInteraction的Type欄位
    public static  class PostInteractionType
    {
        public const string View = "VIEW";
        public const string Like = "LIKE";
        public const string Collect = "COLLECT";
        public const string Share = "SHARE";
        public const string Report = "REPORT";
        public const string Comment = "COMMENT";

    }
}
