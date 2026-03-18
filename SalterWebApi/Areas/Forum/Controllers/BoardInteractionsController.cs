using ForumServiceHelper.IService;
using ForumServiceHelper.Models.DTO.CreateModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SalterWebApi.Areas.Forum.Controllers
{
    [Area("Forum")]
    [Route("api/[area]/[controller]")] // 路由 api/Forum/BoardInteractions
    [ApiController]
    [Tags("社群討論版")]
    public class BoardInteractionsController : ControllerBase
    {
        private readonly IBoardInteractionsService _interactionService;

        public BoardInteractionsController(IBoardInteractionsService interactionService)
        {
            _interactionService = interactionService;
        }

        //看板互動Api
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> HandleInteraction([FromBody] BoardInteractionCreateModel dto)
        {
            var claimId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(claimId) || !int.TryParse(claimId, out int userId))
            {
                return Unauthorized("無效的使用者身分");
            }

            var result = await _interactionService.ProcessInteractionAsync(userId, dto);
            return Ok(result);

        }


    }
}
