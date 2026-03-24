using ExpServiceHelper.DTO;
using SalterEFModels.EFModels;
using FluentEcpay;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpServiceHelper.IService;

namespace ExpServiceHelper.Service
{
    public class SEcPay: ISEcPay, IEquatable<SEcPay>
    {
        #region DI 
        private readonly SalterDbContext _context;
        public SEcPay(SalterDbContext context) {_context = context;}
        #endregion

        public async Task<DAPIResponse<string>> GetPaymentForm(DPaymentRequest dto) {
            
            var transac = await _context.ExpTransactions.FindAsync(dto.TransactionId);
            if (transac == null) return new DAPIResponse<string> {
                IsSuccess = false, Message = "查無該筆交易" };

            //設定綠界的身分證 (你的金鑰)--套件提供的入口點
            //給.WithItems用
            var items = new List<Item> {
                    new Item{
                        Name = dto.ItemName,
                        Price = (int)transac.Amount,
                    }
            };
            var config = new PaymentConfiguration();
            var payment = config.Send.ToApi("https://payment-stage.ecpay.com.tw/Cashier/AioCheckOut/V5")
                            .Send.ToMerchant("2000132") // MerchantID
                            .Send.UsingHash("5294y06JbISpM5x9", "v77hoKGq4kWxJt9M") // HashKey, HashIV
                            .Return.ToServer($"{dto.BaseUrl}/PayResult")
                            .Return.ToClient($"{dto.BaseUrl}/Finish")
                            .Transaction.New(
                                    no:  transac.Id.ToString().PadLeft(10, '0'),
                                description: $"{dto.TransType}",
                                date: DateTime.Now)
                            .Transaction.UseMethod(EPaymentMethod.CVS)
                            .Transaction.WithItems(items)
                            .Generate();//封裝

            //轉成 HTML 表單
            string htmlForm = GenerateHtmlForm(payment);

            return new DAPIResponse<string>
            {
                IsSuccess = true,
                Message = "ECPay繳費單生成",
                Data = htmlForm
            };
        }

        #region  檢查是不是「同一筆交易」
        public string MerchantTradeNo { get; set; }
        public decimal TotalAmount { get; set; }

        public bool Equals(SEcPay? other)
        {
            // 1. 如果對方是空的，那一定不相等
            if (other == null) return false;

            // 2. 定義你的規則：如果交易編號一樣，我們就認為是同一個物件
            return this.MerchantTradeNo == other.MerchantTradeNo;
        }

        // 小教練溫馨提醒：實作 Equals 時，通常也會建議覆寫 Object 的 Equals 和 GetHashCode
        public override bool Equals(object? obj) => Equals(obj as SEcPay);
        public override int GetHashCode() => MerchantTradeNo?.GetHashCode() ?? 0;
        #endregion


        #region 包成html
        private string GenerateHtmlForm(IPayment payment)
        {
            // 這裡我們用 StringBuilder 來拼湊 HTML
            var sb = new StringBuilder();
            sb.AppendLine($"<form id='ecpay-form' action='{payment.URL}' method='post'>");

            // 反射 (Reflection) 抓取所有屬性，自動產生 input
            foreach (var prop in payment.GetType().GetProperties())
            {
                var value = prop.GetValue(payment);
                if (value != null)
                {
                    sb.AppendLine($"<input type='hidden' name='{prop.Name}' value='{value}' />");
                }
            }

            sb.AppendLine("</form>");
            sb.AppendLine("<script type='text/javascript'>document.getElementById('ecpay-form').submit();</script>");

            return sb.ToString();
        }
        #endregion


        #region  
        #endregion
    }
}
