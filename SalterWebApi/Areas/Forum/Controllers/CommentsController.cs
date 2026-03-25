using ForumServiceHelper.IService;
using ForumServiceHelper.Models.DTO.CreateModel;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SalterWebApi.Areas.Forum.Controllers
{
    [Area("Forum")]
    [Route("api/[area]/[controller]")] // 路由 api/Forum/Comments
    [ApiController]
    [Tags("社群討論版")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentsService _commentsService;
        public CommentsController(ICommentsService commentsService)
        {
            _commentsService = commentsService;
        }

        // POST api/<CommentsController>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateComments([FromBody] CommentsCreateModel dto)
        {
            int userId = GetUserId();
            int result =  await _commentsService.CreateCommentAsync(userId,dto);
            return Ok(result);
        }

        // PUT api/<CommentsController>/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateComments(int id, [FromBody] CommentsCreateModel dto)
        {
            int userId = GetUserId();
            bool result = await _commentsService.UpdateCommentAsync(userId,id,dto);
            return Ok(new { Success = true, Message = "留言修改成功" });
        }

        // DELETE api/<CommentsController>/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteComments(int id)
        {
            int userId = GetUserId();
            await _commentsService.DeleteCommentAsync(userId, id);
            return NoContent();
        }

        private int GetUserId()
        {
            var claimId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(claimId) || !int.TryParse(claimId, out int userId))
            {
                throw new UnauthorizedAccessException("無效的使用者身分");
            }

            return userId;
        }
    }
}
