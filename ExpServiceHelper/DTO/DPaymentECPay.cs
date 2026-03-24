using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpServiceHelper.DTO
{
    public class DPaymentECPay
    {
         public int? ECId { get; set; }

        public int? TransactionId { get; set; }

        public string? TradeNo { get; set; }

        public string? MerchantTradeNo { get; set; }

        public DateTime? MerchantTradeDate { get; set; }

        public int? TotalAmount { get; set; }

        public string? TradeDesc { get; set; }

        public string? ItemName { get; set; }

        public int? RtnCode { get; set; }

        public string? RtnMsg { get; set; }

        public DateTime? PaymentDate { get; set; }
    }
}
