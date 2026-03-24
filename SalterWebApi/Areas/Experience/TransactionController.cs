using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ExpServiceHelper.DTO;
using ExpServiceHelper.IService;
using FluentEcpay;

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
        public TransactionController(ISEcPay sEcPay) { 
            _sECpay = sEcPay;
        }
        #endregion


    }
}
