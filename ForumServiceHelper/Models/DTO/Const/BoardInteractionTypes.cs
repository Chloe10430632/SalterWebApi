using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumServiceHelper.Models.DTO.Const
{
    // 定義看板互動類型的常數，對應資料庫中的BoardInteraction的Type欄位
    public static class BoardInteractionTypes
    {
        public const string View = "VIEW";
        public const string Follow = "FOLLOW";
    }
}
