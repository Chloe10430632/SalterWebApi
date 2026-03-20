using ForumServiceHelper.IService;
using ForumServiceHelper.Models.DTO.CreateModel;
using ForumServiceHelper.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SalterWebApi.Areas.Forum.Controllers
{
    [Area("Forum")]
    [Route("api/[area]/[controller]")] // 路由 api/Forum/PostInteractions
    [ApiController]
    [Tags("社群討論版")]
    public class PostInteractionsController : ControllerBase
    {
        private readonly IPostInteractionsService _interactionService;

        public PostInteractionsController(IPostInteractionsService interactionService)
        {
            _interactionService = interactionService;
        }

        //貼文互動Api
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> HandleInteraction([FromBody] PostInteractionCreateModel dto)
        {
            var claimId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(claimId) || !int.TryParse(claimId, out int userId))
            {
                userId = 0;
            }

            var result = await _interactionService.ProcessInteractionAsync(userId, dto);
            return Ok(result);

        }

        
    }
}
