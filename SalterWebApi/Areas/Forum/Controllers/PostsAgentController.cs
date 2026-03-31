using Azure;
using Azure.AI.Extensions.OpenAI;
using Azure.AI.Projects;
using Azure.Identity;
using ForumServiceHelper.Models.DTO.CreateModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Responses;
using System.Text;

namespace SalterWebApi.Areas.Forum.Controllers
{
    [Area("Forum")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Tags("社群討論版")]
    public class PostsAgentController : ControllerBase
    {
        private readonly AIProjectClient _projectClient;
        private readonly string _agentName;
        private string _agentID;

        //透過建構子方法把appsettings的Agent資訊帶進來
        public PostsAgentController(IConfiguration config)
        {
            string endpoint = config["Agent:EndPoint"]!;
            _agentName = config["Agent:AgentName"]!;
        
            _projectClient = new AIProjectClient(
                new Uri(endpoint),
                new DefaultAzureCredential()
                );
        }

        //取得目前的AgentID
        private async Task<string> GetAgentIdAsync()
        {
            if (_agentID is null)
            {
                var agentResponse = await _projectClient.Agents.GetAgentAsync(_agentName);
                _agentID = agentResponse.Value.Id;
            }

            return _agentID;
        }


        //建立對話通道，使用者按下按鈕回傳一個ConversationId的對話通道
        //GET https://localhost:7125/api/chat/start
        [HttpGet("start")]
        public async Task<IActionResult> StartAsync()
        {
            //開啟新對話，像是打電話給agent，回傳對話通道id
            var conversationClient = _projectClient.OpenAI.Conversations;
            var createConverationResponse = await conversationClient.CreateProjectConversationAsync();
            string ConversationId = createConverationResponse.Value.Id;

            return Ok(new { ConversationId = ConversationId });
        }

        //電話打通了，真正傳送資料過去，開始對話
        [HttpPost]
        public async Task<IActionResult> SendAsync([FromBody] SendMsgRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserMessage))
                throw new ArgumentException("請先輸入文案，才能進行優化喔!");

            string agentId = await GetAgentIdAsync();
            var responseClient = _projectClient.OpenAI.GetProjectResponsesClientForAgent(
            new AgentReference(agentId),
            defaultConversationId: request.ConversationId
                 );

            //發送訊息給agent
            var responseResult = await responseClient.CreateResponseAsync(request.UserMessage);
            var responseStatus = responseResult.Value;

            //輪詢
#pragma warning disable OPENAI001
            while (responseStatus.Status == ResponseStatus.InProgress || responseStatus.Status == ResponseStatus.Queued)
            {
                await Task.Delay(2000);
                responseStatus = (await responseClient.GetResponseAsync(responseStatus.Id)).Value;
            }

            //agent回覆完成，讀取資料
            StringBuilder sb = new StringBuilder(); //用來裝agent的回覆
            if (responseStatus.Status == ResponseStatus.Completed)
            {
                foreach (var item in responseStatus.OutputItems)
                {
                    if (item is MessageResponseItem msgItem)
                    {
                        foreach (ResponseContentPart content in msgItem.Content)
                        {
                            if (!string.IsNullOrWhiteSpace(content.Text))
                                sb.AppendLine(content.Text);
                        }
                    }
                }
            }
            else if (responseStatus.Status == ResponseStatus.Failed)
            {
                sb.AppendLine(responseStatus.Error.ToString());

                return Ok(new
                {
                    ConversationId = request.ConversationId,
                    userMessage = request.UserMessage,
                    agentMessage = sb.ToString().Trim()
                });
            }
            return Ok(new
            {
                ConversationId = request.ConversationId,
                userMessage = request.UserMessage,
                agentMessage = sb.ToString().Trim()
            });
#pragma warning restore OPENAI001

        }

        //DTO => Data Transfer Object
        //public record SendMsgRequest(string ConversationId, string UserMessage);
        public record ErrorResponse(int code, string UserMessage, List<string> details);


    }
}
