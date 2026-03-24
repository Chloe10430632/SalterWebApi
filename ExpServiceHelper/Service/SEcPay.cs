using ExpServiceHelper.DTO;
using SalterEFModels.EFModels;
using FluentEcpay;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpServiceHelper.Service
{
    public class SEcPay
    {
        #region DI 
        private readonly SalterDbContext _context;
        public SEcPay(SalterDbContext context) {_context = context;}
        #endregion

        public async Task<DAPIResponse<string>> GetPaymentForm(int transactionId) {
            
            var transac = await _context.ExpTransactions.FindAsync(transactionId);
            if (transac == null) return new DAPIResponse<string> {
                IsSuccess = false, Message = "查無該筆交易" };

            //設定綠界的身分證 (你的金鑰)--套件提供的入口點
            //給.WithItems用
            var items = new List<Item> {
                    new Item{
                        Name = "體驗活動費用",
                        Price = (int)transac.Amount
                    }
            };
            var config = new PaymentConfiguration();
            var payment = config.Send.ToApi("https://payment-stage.ecpay.com.tw/Cashier/AioCheckOut/V5")
                            .Send.ToMerchant("2000132") // MerchantID
                            .Send.UsingHash("5294y06JbISpM5x9", "v77hoKGq4kWxJt9M") // HashKey, HashIV
                            .Return.ToServer("https://你的網址/api/Experience/Checkout/PayResult")
                            .Return.ToClient("https://你的網址/Experience/Checkout/Finish")
                            .Transaction.New(
                                    no: "Exp" + transac.Id.ToString().PadLeft(10, '0'),
                                description: "體驗課程預約",
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
