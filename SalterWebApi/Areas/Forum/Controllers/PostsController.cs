using ForumServiceHelper.IService;
using ForumServiceHelper.Models.DTO.Const;
using ForumServiceHelper.Models.DTO.CreateModel;
using ForumServiceHelper.Models.DTO.ErrorMessage;
using ForumServiceHelper.Models.DTO.ViewModel;
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
        public async Task<ActionResult<IList<PostsViewModel>>> GetForumPosts(
        [FromQuery] int? id,
        [FromQuery] string? keyword,
        [FromQuery] string? sortBy,
        [FromQuery] int? userId)
        {
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                sortBy = sortBy.ToUpper().Trim();
                if (sortBy== SortTypes.Follow && !userId.HasValue)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Code = 400,
                        Message = $"請先登入!"
                    });
                }
            }

            var allPostList = await _postsService.GetAllPostsAsync(id,keyword,sortBy,userId);
            if (allPostList.Count==0)
            {
                return NotFound(new ErrorResponse
                {
                    Code = 404,
                    Message = $"搜尋條件查無相關內容!"
                });
            }

            return Ok(allPostList);
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostsViewModel>> GetForumPost(int id)
        {
            var allPost = await _postsService.GetAllPostsAsync(id);
            var singlePost = allPost.FirstOrDefault();
            if (singlePost == null)
            {
                return NotFound(new ErrorResponse
                {
                    Code = 404,
                    Message = $"找不到 ID 為 {id} 的貼文"
                });
            }
            return Ok(singlePost);
        }

        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutForumPost(int id, ForumPost forumPost)
        {
            return Ok();
            //if (id != forumPost.PostId)
            //{
            //    return BadRequest();
            //}

            //_context.Entry(forumPost).State = EntityState.Modified;

            //try
            //{
            //    await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!ForumPostExists(id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            //return NoContent();
        }

        // POST: api/Posts 有檔案上傳
        [HttpPost]
        public async Task<ActionResult<ForumPost>> PostForumPost([FromForm]PostCreateModel data)
        {
            //先檢查有資料
            if (string.IsNullOrEmpty(data.Content))
                {
                    return BadRequest(new ErrorResponse
                    {
                        Code = 400,
                        Message = "貼文資料不完整",
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
            catch(Exception ex)
            {
                return StatusCode(500, "系統繁忙中，請稍後再試");
            }
        }

        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForumPost(int id)
        {
            return Ok();
            //    var forumPost = await _context.ForumPosts.FindAsync(id);
            //    if (forumPost == null)
            //    {
            //        return NotFound();
            //    }

            //    _context.ForumPosts.Remove(forumPost);
            //    await _context.SaveChangesAsync();

            //    return NoContent();
            //}

            //private bool ForumPostExists(int id)
            //{
            //    return _context.ForumPosts.Any(e => e.PostId == id);
            //} 
        }

    }
}
