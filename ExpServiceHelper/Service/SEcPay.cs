using ExpServiceHelper.DTO;
using ExpServiceHelper.IService;
using FluentEcpay;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using SalterEFModels.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpServiceHelper.Service
{
    public class SEcPay: ISEcPay
    {
        #region DI 
        private readonly SalterDbContext _context;
        private readonly IConfiguration _config;
        public SEcPay(SalterDbContext context, IConfiguration config) {_context = context; _config = config; }
        #endregion

        #region 結帳
        public async Task<DAPIResponse<string>> GetPaymentForm(DTransacRequest dto) {
            
            var merchantId = _config["ECPay:MerchantID"];
            var hashKey = _config["ECPay:HashKey"];
            var hashIV = _config["ECPay:HashIV"];
            var serviceUrl = _config["ECPay:ServiceUrl"];

            var transac = await _context.ExpTransactions.FindAsync(dto.TransactionId);
            if (transac == null) return new DAPIResponse<string> {
                IsSuccess = false, Message = "查無該筆交易" };

            //給.WithItems用
            var items = new List<Item> {
                    new Item{
                        Name = dto.ItemName,
                        Price = (int)transac.Amount,
                    }
            };

            //設定綠界的身分證 (你的金鑰)--套件提供的入口點
            var config = new PaymentConfiguration();
            var payment = config.Send.ToApi(serviceUrl)
                            .Send.ToMerchant(merchantId) // MerchantID
                            .Send.UsingHash(hashKey, hashIV) // HashKey, HashIV
                            .Return.ToServer($"{dto.BaseUrl}/PayResult")
                            .Return.ToClient($"{dto.BaseUrl}/Finish")
                            .Transaction.New(
                                    no: $"{transac.Id}{DateTime.Now:yyyyMMddHHmmss}",
                                description: $"{dto.TransType}",
                                date: DateTime.Now)
                            .Transaction.UseMethod(EPaymentMethod.Credit)
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
        #endregion

        #region 付款結果驗證
        public bool CheckMacValue(Dictionary<string, string> data)
        {
            var hashKey = _config["ECPay:HashKey"];
            var hashIV = _config["ECPay:HashIV"];

            var sorted = data
                .Where(x => x.Key != "CheckMacValue")
                .OrderBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Value);

            var raw = $"HashKey={hashKey}&" +
                      string.Join("&", sorted.Select(x => $"{x.Key}={x.Value}")) +
                      $"&HashIV={hashIV}";

            var encoded = System.Web.HttpUtility.UrlEncode(raw).ToLower();

            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(encoded));
            var result = BitConverter.ToString(bytes).Replace("-", "").ToUpper();

            return result == data["CheckMacValue"];
        }
        #endregion

        #region   存結果並更新 ExpTransactions 表
        public async Task<bool> UpdateTransacForm(Dictionary<string, string> data)
        {
            try { 
                
                //TransacId
                string merchantTradeNo = data["MerchantTradeNo"];
                // 取得扣除後 14 位日期後的剩餘字串，即為原始流水號
                int transactionId = int.Parse(merchantTradeNo.Substring(0, merchantTradeNo.Length - 14));

                // 1. 將 Dictionary 資料對應到 EF Model
                var payResult = new ExpTthirdPartyPayment {
                    TransactionId = transactionId,
                    MerchantTradeNo = merchantTradeNo,
                    TradeNo = data["TradeNo"],
                    RtnCode = int.Parse(data["RtnCode"]),
                    RtnMsg = data["RtnMsg"],
                    TradeAmt = int.Parse(data["TradeAmt"]),
                    PaymentDate = DateTime.TryParse(data["PaymentDate"], out var pDate) ? pDate : DateTime.Now,
                    PaymentType = data["PaymentType"], 
                    SimulatePaid = int.Parse(data["SimulatePaid"])
                };

                // 2. 更新 ExpTthird_party_payments(ECPay) 表 
                 await _context.ExpTthirdPartyPayments.AddAsync(payResult);

                // 3. 更新 ExpTransactions 表 (變更訂單狀態)
                // 使用剛剛解析出來的流水號去資料庫找這筆交易
                var transT = await _context.ExpTransactions
                            .FirstOrDefaultAsync(t => t.Id == transactionId);

                if (transT != null) {
                    transT.Status = 1;
                    // --- 根據交易類型 (TransType) 更新對應各自的Orders表 ---//
                    if (transT.TypeId == null) return false;
                        int typeId = (int)transT.TypeId;
                    switch (typeId) {
                        case 3:
                            var coachOrder = await _context.ExpCourseOrders
                                            .Where(o => o.ExpTransactionId == transactionId)
                                            .ToListAsync();
                            foreach (var order in coachOrder) { order.Status = 1; }
                            break;

                        //case 1: //會員
                        //    var userOrder = await _context.你的order表
                        //                    .Where(o => o.ExpTransactionId == transactionId)
                        //                    .ToListAsync();
                        //    foreach (var order in 你的order表) { order.Status = 1; }
                        //    break;
                        //case 2: //討論版
                        //    var forumOrder = await _context.你的order表
                        //                    .Where(o => o.ExpTransactionId == transactionId)
                        //                    .ToListAsync();
                        //    foreach (var order in 你的order表) { order.Status = 1; }
                        //    break;
                        //case 4: //房源
                        //    var houseOrder = await _context.你的order表
                        //                    .Where(o => o.ExpTransactionId == transactionId)
                        //                    .ToListAsync();
                        //    foreach (var order in 你的order表) { order.Status = 1; }
                        //    break;


                        default:
                           break;
                    }
                }


                // 4. 儲存變更
                await _context.SaveChangesAsync();
                if (data["RtnCode"] != "1") return false;
                    return true;
            }
            catch (Exception ex) { return false;  }
            
        }
        #endregion       

        #region 結帳結果包成html
        private string GenerateHtmlForm(IPayment payment)
        {
            var builder = new StringBuilder();

            builder.Append($@"<form id='ecpay-form' method='POST' action='{payment.URL}'>");

            // 用反射抓所有屬性（這是關鍵）
            var props = payment.GetType().GetProperties();

            foreach (var prop in props)
            {
                var value = prop.GetValue(payment);

                if (value != null)
                {
                    builder.Append($@"<input type='hidden' name='{prop.Name}' value='{value}' />");
                }
            }

            builder.Append(@"
                            </form>
                            <script>
                                document.getElementById('ecpay-form').submit();
                            </script>");

            return builder.ToString();
        }
        #endregion


        #region  用不到-檢查是不是「同一筆交易」
        //public string MerchantTradeNo { get; set; }
        //public decimal TotalAmount { get; set; }

        //public bool Equals(SEcPay? other)
        //{
        //    // 1. 如果對方是空的，那一定不相等
        //    if (other == null) return false;

        //    // 2. 定義你的規則：如果交易編號一樣，我們就認為是同一個物件
        //    return this.MerchantTradeNo == other.MerchantTradeNo;
        //}

        //// 小教練溫馨提醒：實作 Equals 時，通常也會建議覆寫 Object 的 Equals 和 GetHashCode
        //public override bool Equals(object? obj) => Equals(obj as SEcPay);
        //public override int GetHashCode() => MerchantTradeNo?.GetHashCode() ?? 0;
        #endregion

        #region  
        #endregion
    }
}
