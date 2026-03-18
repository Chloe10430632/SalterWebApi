using ForumServiceHelper.Models.DTO.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumServiceHelper.IService
{
    public interface IAdsService
    {
        public Task<AdsViewModel?> GetActiveAdAsync();
    }
}
