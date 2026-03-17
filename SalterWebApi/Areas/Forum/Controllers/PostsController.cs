using Azure;
using ForumServiceHelper.IService;
using ForumServiceHelper.Models.DTO.Const;
using ForumServiceHelper.Models.DTO.CreateModel;
using ForumServiceHelper.Models.DTO.ErrorMessage;
using ForumServiceHelper.Models.DTO.QueryModel;
using ForumServiceHelper.Models.DTO.ViewModel;
using ForumServiceHelper.Service;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalterEFModels.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
      // [Authorize]
        public async Task<ActionResult<IEnumerable<PostListViewModel>>> GetForumPosts(
       [FromQuery] PostsQueryModel query)
        {
            // 1.防禦性程式碼：確保物件存在
            query ??= new PostsQueryModel();

            var claimId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(claimId, out int userId))
                query.UserId = userId;
            else
                query.UserId = 0;

            var postList = await _postsService.GetAllPostsAsync(query);
            return Ok(postList);
        }

        // GET: api/Posts/10
        [HttpGet("{id}")]
        public async Task<ActionResult<PostDetailViewModel>> GetForumPost(int id)
        {
            var post = await _postsService.GetPostDetailAsync(id);
            if (post == null)
            {
                throw new KeyNotFoundException($"找不到 ID 為 {id} 的貼文");
            }
            return Ok(post);
        }

        [HttpPost("Images")] //有檔案上傳[FromForm]
        //[Authorize]
        public async Task<IActionResult> UploadPostImagesToCloudinary([FromForm] List<IFormFile> files)
        {
            // 1. 基礎檢查
            if (files == null || files.Count == 0)
            {
                throw new ArgumentException("請選擇要上傳的檔案");
            }

            var imageUrls = await _postsService.UploadToCloudinaryAsync(files);
       
            if (imageUrls.Count == 0)
            {
                throw new Exception("圖片上傳失敗!");
            }

            // 4. 回傳網址列表給前端
            // 前端拿到這組 string[] 後，再塞進 PostCreateModel.ImageUrls 
            return Created(string.Empty, imageUrls);
        }

        // POST: api/Posts 
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ForumPost>> PostForumPost([FromBody]PostCreateModel data)
        {
            // 既然有 [Authorize]，這裡的 userId 一定有值
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            data.UserId = userId;

            int postIdOrErrorResult = await _postsService.CheckAndCreateAsync(data);
                return Created(string.Empty, new { isSuccess = true, PostId = postIdOrErrorResult });
        }

        // PUT: api/Posts/5
        [HttpPut("{id}")]
        //[Authorize]
        public async Task<IActionResult> PutForumPost(int id, [FromBody] PostCreateModel data)
        {
            var claimId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(claimId))
                data.UserId = 0;
            else
                data.UserId = int.Parse(claimId);

            int postIdOrErrorResult = await _postsService.CheckAndCreateAsync(data, id);
                return Ok(new { isSuccess = true, PostId = postIdOrErrorResult });
        }

        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForumPost(int id)
        {
            bool result =  await _postsService.CheckAndDeleteAsync(id);
            return NoContent(); 
        }

    }
}
