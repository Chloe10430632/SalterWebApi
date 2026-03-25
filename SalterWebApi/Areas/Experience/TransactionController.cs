using ExpServiceHelper.DTO;
using ExpServiceHelper.IService;
using ExpServiceHelper.Service;
using FluentEcpay;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SalterWebApi.Areas.Experience
{
    [Area("Transac")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Tags("交易")]
    public class TransactionController : ControllerBase
    {
        #region 

        #endregion
        #region DI 
        private readonly ISEcPay _sECpay;
        public TransactionController(ISEcPay sEcPay)
        {
            _sECpay = sEcPay;
        }
        #endregion

        [HttpPost("PayResult")]
        public async Task<IActionResult> PayResult([FromForm] IFormCollection collection)
        {
            // 將 IFormCollection 轉為 Dictionary
            var data = collection.ToDictionary(k => k.Key, v => v.Value.ToString());

            // 呼叫驗證方法
            if (_sECpay.CheckMacValue(data))
            {
                if (data["RtnCode"] == "1") // 1 代表成功
                {
                    // TODO: 使用 DTransacECPay 儲存結果並更新您的 ExpTransactions 表
                    // 這裡可以呼叫您的資料庫處理邏輯

                    return Content("1|OK"); 
                }
            }

            return Content("0|Error");
        }
    }
}
