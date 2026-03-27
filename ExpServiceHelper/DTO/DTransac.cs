using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpServiceHelper.DTO
{
    public class DTransac
    {
        public int TransactionId { get; set; }
        public string? CourseName { get; set; } // 從 ExpCourseTemplates 撈
        public int? CoaId { get; set; }// 從 ExpCourseTemplates 撈
        public decimal? Amount { get; set; }    // 從 ExpTransactions 撈
        public string? Status { get; set; }      // 交易狀態
        public DateTime? OrderDate { get; set; } // 交易時間
        public string OrderDateDisplay => OrderDate?.ToString("yyyy-MM-dd HH:mm"); //顯示用的
    }
}
