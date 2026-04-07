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
using System.Web;
using System.Security.Cryptography;

namespace ExpServiceHelper.Service
{
    public class SECPay : ISECPay
    {
        #region DI 
        private readonly SalterDbContext _context;
        private readonly IConfiguration _config;
        public SECPay(SalterDbContext context, IConfiguration config) { _context = context; _config = config; }
        #endregion

        #region 結帳
        public async Task<DAPIResponse<string>> GetPaymentForm(DTransacRequest dto)
        {
            int fromSource = dto.TypeId;

            var ngrokUrl = _config["ECPay:CallbackUrl"];
            var ClientBackURL = _config["ECPay:ClientBackURL"];


            var merchantId = _config["ECPay:MerchantID"];
            var hashKey = _config["ECPay:HashKey"];
            var hashIV = _config["ECPay:HashIV"];
            var serviceUrl = _config["ECPay:ServiceUrl"];

            var transac = await _context.ExpTransactions.FindAsync(dto.TransactionId);
            if (transac == null) return new DAPIResponse<string>
            {
                IsSuccess = false,
                Message = "查無該筆交易"
            };

            //給.WithItems用
            var items = new List<Item> {
                    new Item{
                                Name = "TestCourse",
                                Price = (int)transac.Amount,
                                Quantity = 1
                            }
            };

            //設定綠界的身分證 (你的金鑰)--套件提供的入口點
            var config = new PaymentConfiguration();
            var payment = config.Send.ToApi(serviceUrl)
                            .Send.ToMerchant(merchantId) // MerchantID
                            .Send.UsingHash(hashKey, hashIV) // HashKey, HashIV    
                            .Return.ToServer($"{ngrokUrl}/api/Transac/Transaction/PayResult")//【ToServer】: 綠界通知你的 Server (背景) 
                            .Return.ToClient($"{ClientBackURL}/transaction/finish?orderId={transac.Id}&amount={transac.Amount}&from={fromSource}") //【ToClient】: 使用者付完款自動導回你的頁面 (前景)  
                            .Transaction.New(
                                    no: $"S{transac.Id}{DateTime.Now:yyMMddHHmmss}",
                                description: dto.Description ?? "SalterOrder",
                                date: DateTime.Now)
                            .Transaction.UseMethod(EPaymentMethod.Credit)
                            .Transaction.WithItems(items)
                            .Generate();//封裝
            Console.WriteLine($"套件產出的 ItemName: {payment.ItemName}");

            //轉成 HTML 表單
            var sb = new StringBuilder();
            sb.Append($"<form id='ecpay-form' method='POST' action='{payment.URL}'>");

            foreach (var prop in payment.GetType().GetProperties())
            {
                if (prop.Name == "URL") continue;

                var value = prop.GetValue(payment)?.ToString();
                if (value != null)
                {
                    // 【關鍵修正】：手動將 C# 屬性名稱轉換為綠界 API 要求的名稱
                    string fieldName = prop.Name switch
                    {
                        "MerchantId" => "MerchantID",
                        "ReturnUrl" => "ReturnURL",
                        "OrderResultUrl" => "OrderResultURL",
                        "ClientBackUrl" => "ClientBackURL",
                        "MerchantTradeNo" => "MerchantTradeNo",
                        "MerchantTradeDate" => "MerchantTradeDate",
                        "PaymentType" => "PaymentType",
                        "TotalAmount" => "TotalAmount",
                        "TradeDesc" => "TradeDesc",
                        "ItemName" => "ItemName",
                        "ChoosePayment" => "ChoosePayment",
                        "CheckMacValue" => "CheckMacValue",
                        "EncryptType" => "EncryptType",
                        _ => prop.Name // 其他欄位保持原樣
                    };

                    sb.Append($"<input type='hidden' name='{fieldName}' value='{value}' />");
                }
            }

            sb.Append("</form>");
            sb.Append("<script>document.getElementById('ecpay-form').submit();</script>");





            return new DAPIResponse<string>
            {
                IsSuccess = true,
                Message = "ECPay繳費單生成",
                Data = sb.ToString()
            };
        }
        #endregion

        #region 付款結果驗證
        //private string EncodeValue(string value)
        //{
        //    // 綠界規格：只 encode 這些字元，其餘保留
        //    return Uri.EscapeDataString(value)
        //           .Replace("%20", "+")
        //           .Replace("%2F", "/")   // ✅ 加這行
        //           .Replace("%3A", ":")   // ✅ 加這行
        //           .Replace("%2D", "-")
        //           .Replace("%5F", "_")
        //           .Replace("%2E", ".")
        //           .Replace("%21", "!")
        //           .Replace("%2A", "*")
        //           .Replace("%28", "(")
        //           .Replace("%29", ")");
        //}

        public bool CheckMacValue(string rawBody)
        {
            var hashKey = _config["ECPay:HashKey"];
            var hashIV = _config["ECPay:HashIV"];

            var pairs = rawBody.Split('&')
                .Select(x => x.Split('=', 2))
                .Where(x => x.Length == 2)
                .Select(x => (Key: x[0], Value: x[1]))
                .ToList();

            if (!pairs.Any(x => x.Key == "CheckMacValue")) return false;
            var ecpayMac = pairs.First(x => x.Key == "CheckMacValue").Value;

            var sorted = pairs
                .Where(x => x.Key != "CheckMacValue")
                //.Where(x => !string.IsNullOrEmpty(x.Value))
                .OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase);

            // ✅ value 重新 encode，空格變 %20，中文變 %XX
            var raw = $"HashKey={hashKey}&" +
           string.Join("&", sorted.Select(x =>
               $"{x.Key}={Uri.EscapeDataString(x.Value)
                   .Replace("%2F", "/")
                   .Replace("%3A", ":")
                   .Replace("%20", "+")}")) +   // ← 空格換回 +
           $"&HashIV={hashIV}";

            var toLower = raw.ToLower();
            Console.WriteLine("ENCODED: " + toLower);

            using var sha = SHA256.Create();
            var result = BitConverter.ToString(
                sha.ComputeHash(Encoding.UTF8.GetBytes(toLower))
            ).Replace("-", "").ToUpper();

            Console.WriteLine("MY CheckMac:    " + result);
            Console.WriteLine("ECPay CheckMac: " + ecpayMac);

            return result == ecpayMac;
        }
        
        #endregion

        #region   存結果並更新 ExpTransactions 表
        public async Task<bool> UpdateTransacForm(Dictionary<string, string> data)
        {
            //確保資料一致性(一起成功or失敗)
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                // 1.先安全取得 MerchantTradeNo
                if (!data.TryGetValue("MerchantTradeNo", out string merchantTradeNo)) return false;

                //新增檢查：如果這筆交易已經成功處理過了，就直接回傳 true，不要重複跑 SQL
                var alreadyPaid = await _context.ExpTthirdPartyPayments
                      .AnyAsync(p => p.MerchantTradeNo == merchantTradeNo && p.RtnCode == 1);
                if (alreadyPaid) return true;

                // 取得扣除後 14 位日期後的剩餘字串，即為原始流水號
                //if (merchantTradeNo.Length < 14) return false;
                //int transactionId = int.Parse(merchantTradeNo.Substring(1, merchantTradeNo.Length - 1 - 13));

                // 假設 merchantTradeNo = "S5100080260402231005"
                // 1. 先去掉開頭的 'S' -> 得到 "5100080260402231005"
                string noWithoutS = merchantTradeNo.Substring(1);

                // 2. 扣掉結尾固定的 12 位日期 -> 得到 "5100080"
                // 不管 ID 是 50 還是 5100080，只要後面日期是 12 位，這行永遠正確
                string idPart = noWithoutS.Substring(0, noWithoutS.Length - 12);

                // 3. 轉成數字
                int transactionId = int.Parse(idPart);


                // 1. 將 Dictionary 資料對應到 EF Model
                var payResult = new ExpTthirdPartyPayment
                {
                    TransactionId = transactionId,
                    MerchantTradeNo = merchantTradeNo,
                    // --- 改用以下安全寫法 ---
                    TradeNo = data.ContainsKey("TradeNo") ? data["TradeNo"] : "TEST_TRADE_NO",
                    RtnCode = data.TryGetValue("RtnCode", out var rCode) ? int.Parse(rCode) : 0,
                    RtnMsg = data.ContainsKey("RtnMsg") ? data["RtnMsg"] : "No Message",
                    TradeAmt = data.TryGetValue("TradeAmt", out var amt) ? int.Parse(amt) : 0,
                    PaymentType = data.ContainsKey("PaymentType") ? data["PaymentType"] : "Credit",
                    SimulatePaid = data.TryGetValue("SimulatePaid", out var sim) ? int.Parse(sim) : 0,
                    // -----------------------
                    PaymentDate = DateTime.TryParse(data.ContainsKey("PaymentDate") ? data["PaymentDate"] : "", out var pDate) ? pDate : DateTime.Now
                };

                // 2. 更新 ExpTthird_party_payments(ECPay) 表 
                await _context.ExpTthirdPartyPayments.AddAsync(payResult);

                // 3. 更新 ExpTransactions 表 (變更訂單狀態)
                // 使用剛剛解析出來的流水號去資料庫找這筆交易
                var transT = await _context.ExpTransactions
                            .FirstOrDefaultAsync(t => t.Id == transactionId);

                if (transT != null)
                {
                    transT.Status = 1;
                    // --- 根據交易類型 (TransType) 更新對應各自的Orders表 ---//
                    if (transT.TypeId == null) return false;
                    int typeId = (int)transT.TypeId;

                    switch (typeId)
                    {
                        case 3:
                            var coachOrder = await _context.ExpCourseOrders
                                            .Where(o => o.ExpTransactionId == transactionId)
                                            .ToListAsync();
                            foreach (var order in coachOrder)
                            {
                                order.Status = 1;
                                order.UpdatedAt = DateTime.Now;
                                // 付款成功才 +1
                                var session = await _context.ExpCourseSessions
                                    .FirstOrDefaultAsync(s => s.Id == order.CourseSessionId);
                                if (session != null) session.CurrentParticipants += 1;
                            }
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
                        case 4: //房源
                            var houseBookings = await _context.HomBookings
                                            .Where(o => o.TransactionsId == transactionId)
                                            .ToListAsync();
                            foreach (var booking in houseBookings) { booking.Status = "1"; booking.UpdateTime = DateTime.Now; }
                            break;


                        default:
                            break;
                    }
                }


                // 4. 儲存變更
                if (!data.TryGetValue("RtnCode", out var rtnCode) || rtnCode != "1")
                    return false;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync(); // 全部成功才存檔
                
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // 失敗了就當沒發生過
                return false;
            }

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
                if (prop.Name == "URL") continue;

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

        //public bool Equals(SECPay? other)
        //{
        //    // 1. 如果對方是空的，那一定不相等
        //    if (other == null) return false;

        //    // 2. 定義你的規則：如果交易編號一樣，我們就認為是同一個物件
        //    return this.MerchantTradeNo == other.MerchantTradeNo;
        //}

        //// 小教練溫馨提醒：實作 Equals 時，通常也會建議覆寫 Object 的 Equals 和 GetHashCode
        //public override bool Equals(object? obj) => Equals(obj as SECPay);
        //public override int GetHashCode() => MerchantTradeNo?.GetHashCode() ?? 0;
        #endregion

        #region  
        #endregion
    }
}
