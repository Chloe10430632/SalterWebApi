using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpServiceHelper.DTO
{
    public class DCourseInfoSearch
    {
        // --- 模板相關 ---
        public string? Title { get; set; }        // 模糊搜尋關鍵字
        public int? Difficulty { get; set; }     // 難度等級
        public decimal? MinPrice { get; set; }    // 價格下限
        public decimal? MaxPrice { get; set; }    // 價格上限

        // --- 場次(Session)相關 ---
        public int? CoachId { get; set; }        // 指定教練
        public DateOnly? StartDate { get; set; }  // 搜尋開始日期
        public DateOnly? EndDate { get; set; }    // 搜尋結束日期
        public string? TimeSlot { get; set; }     // 時段關鍵字

        // 是否只顯示「還有名額」的課程
        public bool OnlyShowAvailable { get; set; } = true;
    }
}
