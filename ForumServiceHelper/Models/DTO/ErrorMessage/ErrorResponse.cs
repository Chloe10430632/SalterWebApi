using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumServiceHelper.Models.DTO.ErrorMessage
{
    public class ErrorResponse
    {
        public int Code { get; set; }          // 錯誤代碼 (例如 400)
        public string? Message { get; set; }     // 給人類看的總結 (例如 "資料填寫錯誤")
        public List<string>? Details { get; set; } // 詳細的清單 (例如 "年紀不能負數", "名字太長")
    }
}
