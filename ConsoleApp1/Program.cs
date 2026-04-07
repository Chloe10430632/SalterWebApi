using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;
using System.Text;
using System.Web;

//// 直接把 S5100141 這筆的原始資料硬寫進來
//var data = new Dictionary<string, string>
//{
//    { "MerchantID", "2000132" },
//    { "MerchantTradeNo", "S5100141260407172725" },
//   { "PaymentDate", "2026/04/07 17:28:05" },  // 確認這裡是空格，不是 +
//    { "PaymentType", "Credit_CreditCard" },
//    { "PaymentTypeChargeFee", "31" },
//    { "RtnCode", "1" },
//    { "RtnMsg", "paid" },
//    { "SimulatePaid", "0" },
//    { "TradeAmt", "1500" },
//    { "TradeDate", "2026/04/07 17:27:22" },     // 這裡也是
//    { "TradeNo", "2604071727228388" },
//    { "CheckMacValue", "C55360C2A8C2456C85DBC4CEB308866B1FF3D7AAA1CD6D4EC3686EB8DF51315B" }
//};

//var hashKey = "5294y06JbISpM5x9";
//var hashIV = "v77hoKGq4kWxNNIS";

//var sorted = data
//    .Where(x => x.Key != "CheckMacValue")
//    .Where(x => !string.IsNullOrEmpty(x.Value))
//    .OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase);

//var parts = sorted.Select(x => $"{x.Key}={EncodeValue(x.Value)}");

//var raw = $"HashKey={hashKey}&" + string.Join("&", parts) + $"&HashIV={hashIV}";

//Console.WriteLine("BEFORE_LOWER: " + raw);

//var toLower = raw.ToLower();
//Console.WriteLine("ENCODED: " + toLower);

//// SHA256（你現在的）
//using var sha = SHA256.Create();
//var resultSHA = BitConverter.ToString(
//    sha.ComputeHash(Encoding.UTF8.GetBytes(toLower))
//).Replace("-", "").ToUpper();

////Console.WriteLine("MY CheckMac:    " + result);
////Console.WriteLine("ECPay CheckMac: C55360C2A8C2456C85DBC4CEB308866B1FF3D7AAA1CD6D4EC3686EB8DF51315B");
////Console.WriteLine("Match: " + (result == "C55360C2A8C2456C85DBC4CEB308866B1FF3D7AAA1CD6D4EC3686EB8DF51315B"));

//static string EncodeValue(string value)
//{
//    return Uri.EscapeDataString(value)
//        .Replace("%2F", "/")
//        .Replace("%3A", ":")
//        .Replace("%2D", "-")
//        .Replace("%5F", "_")
//        .Replace("%2E", ".")
//        .Replace("%21", "!")
//        .Replace("%2A", "*")
//        .Replace("%28", "(")
//        .Replace("%29", ")");
//}


//// MD5
//using var md5 = MD5.Create();
//var resultMD5 = BitConverter.ToString(
//    md5.ComputeHash(Encoding.UTF8.GetBytes(toLower))
//).Replace("-", "").ToUpper();

//Console.WriteLine("SHA256:         " + resultSHA);
//Console.WriteLine("MD5:            " + resultMD5);
//Console.WriteLine("ECPay CheckMac: C55360C2A8C2456C85DBC4CEB308866B1FF3D7AAA1CD6D4EC3686EB8DF51315B");

using System.Security.Cryptography;
using System.Text;

var baseStr = "hashkey=5294y06jbispm5x9&merchantid=2000132&merchanttradeno=s5100141260407172725&paymentdate=2026{SLASH}04{SLASH}07{SPACE}17:28:05&paymenttype=credit_creditcard&paymenttypechargefee=31&rtncode=1&rtnmsg=paid&simulatepaid=0&tradeamt=1500&tradedate=2026{SLASH}04{SLASH}07{SPACE}17:27:22&tradeno=2604071727228388&hashiv=v77hokgq4kwxnnis";

var slashOptions = new[] { "/", "%2f", "%2F" };
var spaceOptions = new[] { "%20", "+", " " };
var target = "C55360C2A8C2456C85DBC4CEB308866B1FF3D7AAA1CD6D4EC3686EB8DF51315B";

foreach (var slash in slashOptions)
    foreach (var space in spaceOptions)
    {
        var candidate = baseStr
            .Replace("{SLASH}", slash)
            .Replace("{SPACE}", space);

        using var sha = SHA256.Create();
        var hash = BitConverter.ToString(
            sha.ComputeHash(Encoding.UTF8.GetBytes(candidate))
        ).Replace("-", "").ToUpper();

        if (hash == target)
        {
            Console.WriteLine($"✅ 找到了！slash={slash}, space={space}");
            Console.WriteLine(candidate);
        }
        else
        {
            Console.WriteLine($"❌ slash={slash}, space={space} -> {hash}");
        }
    }