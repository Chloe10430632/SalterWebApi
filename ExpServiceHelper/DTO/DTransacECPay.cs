using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpServiceHelper.DTO
{
    public class DTransacECPay
    {
       
            public int? ECId { get; set; } // 資料庫主鍵

            public int? TransactionId { get; set; } // 關聯到你的 ExpTransactions

            // --- 以下為綠界回傳欄位 ---

            public string? TradeNo { get; set; } // 綠界交易編號

            public string? MerchantTradeNo { get; set; } // 特店交易編號

            // ⚠️ 修正：綠界回傳的是 TradeAmt 而不是 TotalAmount
            public int? TradeAmt { get; set; }

            public int? RtnCode { get; set; } // 1 為成功

            public string? RtnMsg { get; set; }

            public DateTime? PaymentDate { get; set; }

            // 💡 建議新增：付款方式 (例: Credit_CreditCard)
            public string? PaymentType { get; set; }

            // 💡 建議新增：是否為模擬付款 (1: 是, 0: 否)
            // 這能防止有人在測試環境跑完流程後，偽造請求讓正式環境出貨
            public int? SimulatePaid { get; set; }

            // 💡 建議新增：自訂欄位 (如果你當初有送 CustomField1~4)
            public string? CustomField1 { get; set; }
        
    }
}
