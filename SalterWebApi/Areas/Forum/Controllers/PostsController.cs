using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForumServiceHelper.IService;
using ForumServiceHelper.Models.DTO.ErrorMessage;
using ForumServiceHelper.Models.DTO.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalterEFModels.EFModels;

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
        [HttpGet]
        public async Task<ActionResult<IList<PostsViewModel>>> GetForumPosts(
        [FromQuery] int? id,
        [FromQuery] string? keyword,
        [FromQuery] string? sortBy,
        [FromQuery] int? userId)
        {
            var allPostList = await _postsService.GetAllPostsAsync(id,keyword,sortBy,userId);

            if (allPostList == null)
            {
                return NoContent();
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

        // POST: api/Posts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ForumPost>> PostForumPost(ForumPost forumPost)
        {
            return Ok();
            //_context.ForumPosts.Add(forumPost);
            //await _context.SaveChangesAsync();

            //return CreatedAtAction("GetForumPost", new { id = forumPost.PostId }, forumPost);
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
