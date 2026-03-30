using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumServiceHelper.Models.DTO.Response
{
    public class SensitiveWordsResponseModel
    {
        public bool IsValid { get; set; }
        public List<string> ViolatedWords { get; set; } = new();
    }
}
