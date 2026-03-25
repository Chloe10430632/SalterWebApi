using ExpServiceHelper.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpServiceHelper.IService
{
    public interface ISEcPay
    {
       /**按結帳*/
        Task<DAPIResponse<string>> GetPaymentForm(DTransacRequest dto);
        /**驗證*/
        public bool CheckMacValue(Dictionary<string, string> data);
       /**交易成功改狀態*/
        Task<bool> UpdateTransacForm(Dictionary<string, string> data);
    }
}
