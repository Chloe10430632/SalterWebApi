using ForumServiceHelper.Models.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumServiceHelper.IService
{
    public interface ISensitiveWordsService
    {
        public Task<SensitiveWordsResponseModel> ValidateContentAsync(string content);
    }
}
