using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ForumRepositoryHelper.IRepository;
using ForumServiceHelper.IService;
using ForumServiceHelper.Models.DTO.Const;
using ForumServiceHelper.Models.DTO.CreateModel;
using ForumServiceHelper.Models.DTO.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SalterEFModels.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumServiceHelper.Service
{
    public class PostsService:IPostsService
    {
        private readonly IGenericSalterRepository<ForumPost> _dbPosts;
        private readonly string _cloudinaryName;
        private readonly string _cloudinaryApiKey;
        private readonly string _cloudinaryApiSecret;

        public PostsService(IGenericSalterRepository<ForumPost> dbPosts,IConfiguration config)
        {
            _dbPosts = dbPosts;
            _cloudinaryName = config["Cloudinary:CloudName"];
            _cloudinaryApiKey = config["Cloudinary:ApiKey"];
            _cloudinaryApiSecret = config["Cloudinary:ApiSecret"];
        }

        public async Task<IList<PostsViewModel>> GetAllPostsAsync(int? postId = null, string? keyword = null, string? sortBy = null, int? userId = null)
        {
            //1.強制指定變數類型為 IQueryable < ForumPost >，這樣後續的 Where 才能順利對接
            IQueryable<ForumPost> posts = _dbPosts.GetDbContext().ForumPosts
                .Include(p => p.User)
                .Include(p => p.Board)
                .Include(p => p.ForumPostsImages)
                .Include(p => p.ForumPostInteractions)
                .Include(p => p.ForumComments);

            //預設排序按照瀏覽量
            sortBy ??= SortTypes.Popular; 
            sortBy = sortBy.ToUpper().Trim();


            //現在 posts 是 IQueryable，你可以自由地疊加過濾條件
            if (postId.HasValue)
            {
                posts = posts.Where(p => p.PostId == postId.Value);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.Trim();
                posts = posts.Where(p => p.Content.Contains(keyword) || p.Board.Title.Contains(keyword));
            }

            if (userId.HasValue && sortBy == SortTypes.Follow)
            {
                //先找出userId追蹤的所有看板
                var userFollowBoards = _dbPosts.GetDbContext().ForumBoardInteractions
                    .Where(bi => bi.UserId == userId && bi.Type == BoardInteractionTypes.Follow)
                    .Select(bi => bi.BoardId);

                // 只留下屬於這些看板的貼文
                posts = posts.Where(p => userFollowBoards.Contains(p.BoardId));
            }

            // 計算貼文距離現在的發佈時間 (先以基準日呈現)
            DateTime now = new DateTime(2026, 2, 12, 12, 35, 0); //DateTime.Now
            var postDetails = posts.Select(p => new PostsViewModel
            {
                PostId = p.PostId,
                UserName = p.User.UserName,
                AvatarUrl = p.User.ProfilePicture,
                BoardTitle = p.Board.Title,
                Content = p.Content,
                ImageUrls = p.ForumPostsImages
                 .Where(img => img.PostId == p.PostId)
                 .Select(img => img.ImageUrl).ToList(),

                // 計算時間差邏輯
                CreatedAt = p.CreatedAt,
                AgoMinuteNumber = (int)(now - p.CreatedAt).TotalMinutes,
                AgoHourNumber = (int)(now - p.CreatedAt).TotalHours,
                AgoDayNumber = (int)(now - p.CreatedAt).TotalDays,

                LikeCount = p.ForumPostInteractions
                 .Where(pi => pi.PostId == p.PostId && pi.Type == PostInteractionType.Like)
                 .Count(),

                CommentCount = p.ForumComments
                 .Where(pi => pi.PostId == p.PostId)
                 .Count(),

                BookmarkCount = p.ForumPostInteractions
                 .Where(pi => pi.PostId == p.PostId && pi.Type == PostInteractionType.Collect)
                 .Count(),

                ShareCount = p.ForumPostInteractions
                 .Where(pi => pi.PostId == p.PostId && pi.Type == PostInteractionType.Share)
                 .Count(),

                ViewCount = p.ForumPostInteractions
                 .Where(pi => pi.PostId == p.PostId && pi.Type == PostInteractionType.View)
                 .Count(),

                // 處理父子留言結構
                Comments = p.ForumComments
                .Where(c => c.ParentCommentId == null) // 先過濾出「父留言」
                .OrderByDescending(c => c.CreatedAt)
                // .Take(2) // 如果只要顯示最新兩則大標留言
                .Select(c => new CommentPreviewDto
                {
                    CommentId = c.CommentId,
                    UserName = c.User.UserName,
                    Content = c.Content,
                    AvatarUrl = c.User.ProfilePicture,
                    CreatedAt = c.CreatedAt,

                    // 2. 找出屬於這個父留言的子留言
                    Replies = p.ForumComments
                        .Where(reply => reply.ParentCommentId == c.CommentId)
                        .OrderBy(reply => reply.CreatedAt) // 子留言通常由舊到新排
                        .Select(reply => new CommentPreviewDto
                        {
                            CommentId = reply.CommentId,
                            UserName = reply.User.UserName,
                            Content = reply.Content,
                            AvatarUrl = reply.User.ProfilePicture,
                            CreatedAt = reply.CreatedAt
                        }).ToList()
                }).ToList(),

                PostTags = _dbPosts.GetDbContext().ForumPostTagDetails
                     .Include(pt => pt.Tag)
                     .Where(pt => pt.PostId == p.PostId)
                     .Select(pt =>  pt.Tag.TagName )
                     .ToList()
                });

                if (sortBy == SortTypes.Popular || sortBy == SortTypes.Follow)
                {
                    postDetails = postDetails.OrderByDescending(pd=>pd.ViewCount);
                }

                if(sortBy == SortTypes.New)
                {
                    postDetails = postDetails.OrderByDescending(pd => pd.CreatedAt);
                }

            return await postDetails.ToListAsync();
        }

        public async Task<int> CheckAndCreate(PostCreateModel data)
        {
            List<string> finalImageUrls = new List<string>();

            if (data != null)
            {
                // 1. 建立貼文主體
                var newPost = new ForumPost
                {
                    UserId = data.UserId,
                    BoardId = data.BoardId,
                    Content = data.Content,
                    LocationId = data.LocationId,
                    Status = data.Status,
                    IsPinned = false,
                    IsPosted = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _dbPosts.Add(newPost);
                bool isPostSaved = await _dbPosts.SaveChangesAsync();

                if (isPostSaved)
                {
                    // 2. 處理圖片
                    if (data.Images.Any())
                    {
                        foreach (var file in data.Images)
                        {
                            if (file.Length > 0)
                            {
                                // 建立 Cloudinary 帳戶實例 (建議從設定檔讀取或 DI 注入)
                                var account = new Account(_cloudinaryName, _cloudinaryApiKey, _cloudinaryApiSecret);
                                var cloudinary = new Cloudinary(account);

                                // 使用資料流讀取上傳圖片
                                using var stream = file.OpenReadStream();
                                var uploadParams = new ImageUploadParams()
                                {
                                    File = new FileDescription(file.FileName, stream),
                                    Folder = "Salter/Forum",
                                    PublicId = $"{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString().Substring(0, 4)}",
                                    // 可加入圖片轉換設定 (例如限定大小)
                                    Transformation = new Transformation().Quality("auto").FetchFormat("auto")
                                };

                                // 執行非同步上傳
                                var uploadResult = cloudinary.Upload(uploadParams);
                                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    // 取得 Cloudinary 回傳的 SecureUrl (https)
                                    string uploadedUrl = uploadResult.SecureUrl.ToString();
                                    finalImageUrls.Add(uploadedUrl);
                                    // 將相對路徑存入資料庫 (供前端存取使用)
                                    var postImage = new ForumPostsImage
                                    {
                                        PostId = newPost.PostId,
                                        ImageUrl = uploadedUrl
                                    };
                                    _dbPosts.GetDbContext().Add(postImage);
                                    await _dbPosts.SaveChangesAsync();


                                }
                                else
                                {
                                    return -1;
                                }
                            }
                        }
                    }

                    // 3. 處理標籤關聯 (假設標籤表已經存在標籤)
                    if (data.TagNames.Any())
                    {
                        foreach (var tagName in data.TagNames)
                        {
                            // 找出或新增標籤 (視你的業務邏輯而定)
                            //if(_dbPosts.GetDbContext().ForumTags.Any(t=>t.TagName == tagName))
                            //{

                            //}
                            // 這裡簡單示範：直接存入細節表
                            // 需要先確認 Tag 表裡有沒有這個 TagName...
                        }
                    }

                    //await db.SaveChangesAsync();

                    return newPost.PostId; // 回傳 ID 讓前端可以跳轉
                }



                // 先 SaveChanges 一次，EF 會自動幫 newPost 填入生成的 PostId
                await _dbPosts.SaveChangesAsync();







            }



            return 1;


        }

    }
}
