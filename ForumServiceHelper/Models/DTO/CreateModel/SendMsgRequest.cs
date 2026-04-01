using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumServiceHelper.Models.DTO.CreateModel
{
    public class SendMsgRequest
    {
        public string? ConversationId { get; set; }
        public string? UserMessage { get; set; }
        public string? AgentMessage { get; set; }
    }
}
