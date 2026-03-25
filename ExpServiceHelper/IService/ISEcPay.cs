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
        Task<DAPIResponse<string>> GetPaymentForm(DTransacRequest dto);
        public bool CheckMacValue(Dictionary<string, string> data);
    }
}
