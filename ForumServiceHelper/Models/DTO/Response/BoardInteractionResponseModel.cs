using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumServiceHelper.Models.DTO.Response
{
    public class BoardInteractionResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string CurrentStatus { get; set; } = string.Empty; //改變按鈕狀態
    }
}
