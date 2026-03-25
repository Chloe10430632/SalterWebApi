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

        [Authorize]
        [HttpPost("PayResult")]
        public async Task<IActionResult> PayResult([FromForm] IFormCollection collection)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdStr))
            {
                return Unauthorized(new { message = "無效的憑證，請重新登入" });
            }

            if (int.TryParse(userIdStr, out int currentUserId))
            { 
                // 將 IFormCollection 轉為 Dictionary
                var data = collection.ToDictionary(k => k.Key, v => v.Value.ToString());

            // 呼叫驗證方法
            if (_sECpay.CheckMacValue(data))
            {
                if (data["RtnCode"] == "1") // 1 代表成功
                {
                    //DB更新
                    await _sECpay.UpdateTransacForm(data);

                    return Content("1|OK"); 
                }
            }
        }
            return Content("0|Error");}
    }
}
