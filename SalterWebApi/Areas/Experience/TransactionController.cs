using ExpServiceHelper.DTO;
using ExpServiceHelper.IService;
using ExpServiceHelper.Service;
using FluentEcpay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SalterWebApi.Areas.Experience
{
    [Area("Transac")]
    [Route("api/Transac/[controller]")]
    [ApiController]
    [Tags("交易")]
    public class TransactionController : ControllerBase
    {
        #region 

        #endregion
        #region DI 
        private readonly ISECPay _sECpay;
        public TransactionController(ISECPay sEcPay)
        {
            _sECpay = sEcPay;
        }
        #endregion


        #region 前端收單 
        [Authorize]
        [HttpPost("GetOrderForm")] //測完用post以ts黨設定傳入資料
        //[HttpGet("GetOrderForm")]
        //public async Task<IActionResult> GetOrderForm([FromQuery] DTransacRequest dto)
        public async Task<IActionResult> GetOrderForm(DTransacRequest dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdStr))
            {
                return Unauthorized(new { message = "無效的憑證，請重新登入" });
            }

                var result = await _sECpay.GetPaymentForm(dto);

                if (!result.IsSuccess)
                    return BadRequest(result);
                return Content(result.Data, "text/html");
            
         
        }
        #endregion

        #region 回報付款結果
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        [HttpPost("PayResult")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> PayResult([FromForm] IFormCollection collection)
        {
            // 將 IFormCollection 轉為 Dictionary
            var data = collection.ToDictionary(k => k.Key, v => v.Value.ToString());
            //  驗證來源是否真的是綠界
            if (!_sECpay.CheckMacValue(data))
            {
                return Content("0|CheckMacValueVerifyFail");
            }

            // 成功才寫入
            if (data["RtnCode"] == "1") // 1 代表成功
                {
                    //DB更新
                    await _sECpay.UpdateTransacForm(data);

                    return Content("1|OK");
                }
            Console.WriteLine($"收到綠界通知：單號={data["MerchantTradeNo"]}, 結果={data["RtnCode"]}");
            return Content("0|Error");
        }
        #endregion

        #region 交易完成 
        [HttpPost("Finish")]
        [Consumes("application/x-www-form-urlencoded")] // 必加，接收返回商店按鈕的 POST
        public IActionResult Finish([FromForm] IFormCollection collection)
        {
            // 這裡通常是跳轉回你的前端 (localhost:4200)
            return Redirect("http://localhost:4200/finish");
        }
        #endregion
    }


}
