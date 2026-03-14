using Azure;
using ForumServiceHelper.IService;
using ForumServiceHelper.Models.DTO.Const;
using ForumServiceHelper.Models.DTO.CreateModel;
using ForumServiceHelper.Models.DTO.ErrorMessage;
using ForumServiceHelper.Models.DTO.QueryModel;
using ForumServiceHelper.Models.DTO.ViewModel;
using ForumServiceHelper.Service;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalterEFModels.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalterWebApi.Areas.Forum.Controllers
{
    [Area("Forum")]
    [Route("api/[area]/[controller]")] // 路由 api/Forum/Posts
    [ApiController]
    [Tags("社群討論版")]
    public class PostsController : ControllerBase
    {
        private readonly IPostsService _postsService;

        public PostsController(IPostsService postsService)
        {
            _postsService = postsService;
        }

        // GET: api/Posts
        //GET: api/Posts?sortBy=popular.....(Query)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostListViewModel>>> GetForumPosts(
       [FromQuery] PostsQueryModel query)
        {
            if (query.TakeSize <= 0) query.TakeSize = 5;
            if (query.TakeSize > 5) query.TakeSize = 5; //最多就是只能抓5筆

            var postList = await _postsService.GetAllPostsAsync(query);

            if(postList.Count() == 0)
            {
                return NotFound(new ErrorResponse
                {
                    Code = 404,
                    Message = $"搜尋條件查無相關內容!"
                });
            }

            return Ok(postList);
        }

        // GET: api/Posts/10
        [HttpGet("{id}")]
        public async Task<ActionResult<PostDetailViewModel>> GetForumPost(int id)
        {
            var post = await _postsService.GetPostDetailAsync(id);
            if (post == null)
            {
                return NotFound(new ErrorResponse
                {
                    Code = 404,
                    Message = $"找不到 ID 為 {id} 的貼文"
                });
            }
            return Ok(post);
        }

        // POST: api/Posts 有檔案上傳
        [HttpPost]
        public async Task<ActionResult<ForumPost>> PostForumPost([FromForm] PostCreateModel data)
        {
            //先檢查有資料
            if (string.IsNullOrEmpty(data.Content))
            {
                return BadRequest(new ErrorResponse
                {
                    Code = 400,
                    Message = "貼文內容不得為空白!",
                });
            }
            //有資料開始處理後端邏輯
            try
            {
                int postIdOrErrorResult = await _postsService.CheckAndCreateAsync(data);
                if (postIdOrErrorResult == -1)
                {
                    return StatusCode(500, new ErrorResponse
                    {
                        Code = 500,
                        Message = "伺服器處理貼文失敗"
                    });
                }

                // return CreatedAtAction(nameof(GetForumPost), new { id = postIdOrErrorResult },data);
                return Ok(new { isSuccess = true, PostId = postIdOrErrorResult });

            }
            catch (Exception ex)
            {
                return StatusCode(500, "系統繁忙中，請稍後再試");
            }
        }

        // PUT: api/Posts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutForumPost(int id, [FromForm] PostCreateModel data)
        {
            //先檢查有資料
            if (string.IsNullOrEmpty(data.Content))
            {
                return BadRequest(new ErrorResponse
                {
                    Code = 400,
                    Message = "貼文內容不得為空白!",
                });
            }

            //有資料開始處理後端邏輯
            try
            {
                int postIdOrErrorResult = await _postsService.CheckAndCreateAsync(data, id);
                if (postIdOrErrorResult == -1)
                {
                    return NotFound(new ErrorResponse
                    {
                        Code = 404,
                        Message = $"找不到 ID 為 {id} 的貼文!"
                    });
                }

                return Ok(new { isSuccess = true, PostId = postIdOrErrorResult });

            }
            catch (Exception ex)
            {
                return StatusCode(500, "系統繁忙中，請稍後再試");
            }
        }

        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForumPost(int id)
        {
            bool result =  await _postsService.CheckAndDeleteAsync(id);

            if (!result)
                return NotFound(new ErrorResponse
                {
                    Code = 404,
                    Message = $"找不到 ID 為 {id} 的貼文"
                });
            return NoContent(); 
        }

    }
}
