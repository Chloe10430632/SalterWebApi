using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpServiceHelper.DTO
{
    public class DPaymentRequest
    {
        // 1. 交易 ID (從你的資料庫來的)
        public int TransactionId { get; set; }

        // 2. 商品名稱 (例如："自由潛水教練課" 或 "衝浪板租借")
        public string ItemName { get; set; }

        // 3. 訂單描述 (顯示在綠界頁面上)
        public string Description { get; set; }

        // 4. 你的 API 基礎路徑 (因為每個人的 Area 或 Controller 可能不同)
        // 例如：https://localhost:7000/Experience/Coach
        public string BaseUrl { get; set; }
        
        public string TransType { get; set; }
    }
}
