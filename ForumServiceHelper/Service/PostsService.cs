using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ForumRepositoryHelper.IRepository;
using ForumServiceHelper.IService;
using ForumServiceHelper.Models.DTO.Const;
using ForumServiceHelper.Models.DTO.CreateModel;
using ForumServiceHelper.Models.DTO.QueryModel;
using ForumServiceHelper.Models.DTO.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SalterEFModels.EFModels;
using System.ComponentModel;


namespace ForumServiceHelper.Service
{
    public class PostsService:IPostsService
    {
        private readonly IGenericSalterRepository<ForumPost> _dbPosts;
        private readonly string? _cloudinaryName;
        private readonly string? _cloudinaryApiKey;
        private readonly string? _cloudinaryApiSecret;

        public PostsService(IGenericSalterRepository<ForumPost> dbPosts,IConfiguration config)
        {
            _dbPosts = dbPosts;
            _cloudinaryName = config["Cloudinary:CloudName"];  
            _cloudinaryApiKey = config["Cloudinary:ApiKey"];
            _cloudinaryApiSecret = config["Cloudinary:ApiSecret"];
        }

        public async Task<IEnumerable<PostListViewModel>> GetAllPostsAsync(int userId,PostsQueryModel query)
        {
            query.SortBy = query.SortBy.ToUpper();
            query.TakeSize = (query.TakeSize <= 0 || query.TakeSize > 5) ? 5 : query.TakeSize;
            var posts = _dbPosts.GetAll().Where(p => p.IsPosted && p.Status == PostCreateStatusTypes.Normal);

            if (query.BoardId.HasValue) 
                posts = posts.Where(p => p.BoardId == query.BoardId);

            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                var kw = query.Keyword.Trim();
                posts = posts.Where(p => p.Content.Contains(kw) || p.Board.Title.Contains(kw));
            }

            if(query.SortBy == SortTypes.Follow && userId == 0)
                throw new UnauthorizedAccessException("欲瀏覽追蹤貼文，請先登入喔!");

            //處理 Follow 邏輯(使用 Join 效率更高)
            if (query.SortBy == SortTypes.Follow && userId > 0)
            {
                var followedBoardIds = _dbPosts.GetDbContext().ForumBoardInteractions
                    .Where(bi => bi.UserId == userId && bi.Type == BoardInteractionTypes.Follow)
                    .Select(bi => bi.BoardId);
                posts = posts.Where(p => followedBoardIds.Contains(p.BoardId));
            }

            var postListQuery = posts.Select(p => new PostListViewModel
            {
                PostId = p.PostId,
                UserName = p.User.UserName,
                AvatarUrl = p.User.ProfilePicture,
                BoardId = p.BoardId,
                BoardTitle = p.Board.Title,
                LocationTitle = p.Location.Name,
                ContentPreview = p.Content.Length > 150 ? p.Content.Substring(0, 150) : p.Content,
                CreatedAt = p.CreatedAt,
                ImageUrls = p.ForumPostsImages.OrderBy(img => img.SortIndex).Select(img => img.ImageUrl).ToList(),
                IsLiked = userId > 0 && p.ForumPostInteractions.Any(i => i.UserId == userId && i.Type == PostInteractionType.Like),
                LikeCount = p.ForumPostInteractions.Count(i => i.Type == PostInteractionType.Like),
                IsCollected =  userId > 0 && p.ForumPostInteractions.Any(i => i.UserId == userId && i.Type == PostInteractionType.Collect),
                CollectCount = p.ForumPostInteractions.Count(i => i.Type == PostInteractionType.Collect),
                ShareCount = p.ForumPostInteractions.Count(i => i.Type == PostInteractionType.Share),
                CommentCount = p.ForumComments.Count(),
                ViewCount = p.ForumPostInteractions.Count(i => i.Type == PostInteractionType.View),
                PostTags = p.ForumPostTagDetails.Select(pt => pt.Tag.TagName).ToList()
            });

            // 分頁排序 (以最難的 POPULAR 為例)
            if (query.SortBy.ToUpper() == SortTypes.Popular)
            {
                if (query.LastViewCount.HasValue && query.LastId.HasValue)
                    postListQuery = postListQuery.Where(p => p.ViewCount < query.LastViewCount || (p.ViewCount == query.LastViewCount && p.PostId < query.LastId));

                postListQuery = postListQuery.OrderByDescending(p => p.ViewCount).ThenByDescending(p => p.PostId);
            }
            else // NEW 即時貼文
            {
                if (query.LastCreatedAt.HasValue && query.LastId.HasValue)
                    postListQuery = postListQuery.Where(p => p.CreatedAt < query.LastCreatedAt || (p.CreatedAt == query.LastCreatedAt && p.PostId < query.LastId));

                postListQuery = postListQuery.OrderByDescending(p => p.CreatedAt).ThenByDescending(p => p.PostId);
            }

            return await postListQuery.Take(query.TakeSize).ToListAsync();
        }

        public async Task<PostDetailViewModel?> GetPostDetailAsync(int postId)
        {
            return await _dbPosts.GetAll()
        .Where(p => p.PostId == postId)
        .Select(p => new PostDetailViewModel
        {
            PostId = p.PostId,
            UserName = p.User.UserName,
            AvatarUrl = p.User.ProfilePicture,
            BoardTitle = p.Board.Title,
            ContentPreview = p.Content.Length > 150 ? p.Content.Substring(0, 150) : p.Content,
            CreatedAt = p.CreatedAt,
            ImageUrls = p.ForumPostsImages.Select(img => img.ImageUrl).ToList(),
            LikeCount = p.ForumPostInteractions.Count(i => i.Type == PostInteractionType.Like),
            CommentCount = p.ForumComments.Count(),
            ViewCount = p.ForumPostInteractions.Count(i => i.Type == PostInteractionType.View),
            PostTags = p.ForumPostTagDetails.Select(pt => pt.Tag.TagName).ToList(),

            BookmarkCount = p.ForumPostInteractions.Count(i => i.Type == PostInteractionType.Collect),
            ShareCount = p.ForumPostInteractions.Count(i => i.Type == PostInteractionType.Share),
            FullContent = p.Content,

            // 處理父子留言結構
            Comments = p.ForumComments
                .Where(c => c.ParentCommentId == null)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentPreviewDto
                {
                    CommentId = c.CommentId,
                    UserName = c.User.UserName,
                    Content = c.Content,
                    AvatarUrl = c.User.ProfilePicture,
                    CreatedAt = c.CreatedAt,
                    Replies = p.ForumComments
                        .Where(r => r.ParentCommentId == c.CommentId)
                        .OrderBy(r => r.CreatedAt)
                        .Select(r => new CommentPreviewDto
                        {
                            CommentId = r.CommentId,
                            UserName = r.User.UserName,
                            AvatarUrl = r.User.ProfilePicture,
                            Content = r.Content,
                            CreatedAt = r.CreatedAt,
                        }).ToList()
                }).ToList()
        }).FirstOrDefaultAsync();
        }

        public async Task<List<string>> UploadToCloudinaryAsync(List<IFormFile> images)
        {
            var imageUrls = new List<string>();
            if (images.Count > 0)
            {
                var account = new Account(_cloudinaryName, _cloudinaryApiKey, _cloudinaryApiSecret);
                var cloudinary = new Cloudinary(account);

                int fileIndex = 1;
                foreach (var file in images)
                {
                    if (file.Length <= 0) continue;

                    using var stream = file.OpenReadStream();
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream),
                        Folder = "Salter/Forum",
                        PublicId = $"{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString()[..4]}",
                        Transformation = new Transformation().Quality("auto").FetchFormat("auto")
                    };

                    // 改用 UploadAsync 提升非同步效能
                    var uploadResult = await cloudinary.UploadAsync(uploadParams);
                    if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        imageUrls.Add(uploadResult.SecureUrl.ToString());
                    }
                }
                return imageUrls;
            }
            return imageUrls;
        }

        public async Task<int> CheckAndCreateAsync(int userId, PostCreateModel data, int? postId = null) //AddOrUpdate
        {
            if (string.IsNullOrWhiteSpace(data.Content))
            {
                throw new ArgumentException("貼文內容不可為空，請輸入文字。");
            }

            if (data.BoardId <= 0)
            {
                throw new ArgumentException("請選擇正確的看板。");
            }

            ForumPost? post;

            if (postId.HasValue && postId.Value > 0)
            {
                // 更新模式：包含舊的關聯以便後續處理
                post = await _dbPosts.GetDbContext().ForumPosts
                    .Include(p => p.ForumPostsImages)
                    .Include(p => p.ForumPostTagDetails)
                    .FirstOrDefaultAsync(p => p.PostId == postId.Value);

                if (post == null) throw new KeyNotFoundException($"找不到 ID 為 {postId} 的貼文");

                if(post.UserId!= userId)
                    throw new ArgumentException($"您沒有權限修改他人貼文!");

                // 更新欄位內容
                post.BoardId = data.BoardId;
                post.Content = data.Content;
                post.LocationId = data.LocationId;
                post.IsPosted = data.isPosted;
                post.Status = data.isPosted ? PostCreateStatusTypes.Normal : PostCreateStatusTypes.Hide;
                post.UpdatedAt = DateTime.Now;

                // 清理舊有的圖片與標籤關聯 (採取「先刪後加」策略是最穩健的)
                _dbPosts.GetDbContext().ForumPostsImages.RemoveRange(post.ForumPostsImages);
                _dbPosts.GetDbContext().ForumPostTagDetails.RemoveRange(post.ForumPostTagDetails);
            }
            else
            {
                // 新增模式
                post = new ForumPost
                {
                    UserId = userId,
                    BoardId = data.BoardId,
                    Content = data.Content,
                    LocationId = data.LocationId,
                    IsPinned = false,
                    IsPosted = data.isPosted,
                    Status = data.isPosted ? PostCreateStatusTypes.Normal : PostCreateStatusTypes.Hide,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _dbPosts.Add(post);
            }

            // --- 2. 處理圖片 (直接使用傳入的 URL) ---
            if (data.ImageUrls is { Count: > 0 })
            {
                for (int i = 0; i < data.ImageUrls.Count; i++)
                {
                    var postImage = new ForumPostsImage
                    {
                        Post = post, // EF 會自動處理關聯
                        ImageUrl = data.ImageUrls[i],
                        SortIndex = i + 1
                    };
                    _dbPosts.GetDbContext().ForumPostsImages.Add(postImage);
                }
            }

            // --- 3. 處理標籤 (優化後的邏輯) ---
            if (data.Tags is { Count: > 0 })
            {
                var tagNames = data.Tags.Select(t => t.TagName.Trim()).Distinct().ToList();

                var existingTags = await _dbPosts.GetDbContext().ForumTags
                    .Where(t => tagNames.Contains(t.TagName))
                    .ToListAsync();

                foreach (var name in tagNames)
                {
                    var targetTag = existingTags.FirstOrDefault(t => t.TagName == name);

                    if (targetTag == null)
                    {
                        targetTag = new ForumTag { TagName = name };
                        _dbPosts.GetDbContext().ForumTags.Add(targetTag);
                    }

                    var detail = new ForumPostTagDetail
                    {
                        Post = post,
                        Tag = targetTag
                    };
                    _dbPosts.GetDbContext().ForumPostTagDetails.Add(detail);
                }
            }

            // --- 4. 存檔 ---
            await _dbPosts.SaveChangesAsync();
            return post.PostId;
        }

        public async Task<bool> CheckAndDeleteAsync(int userId,int postId)
        {
            // 1. 抓出貼文主體，同時 Include 所有關聯表
            // 把關聯資料一併載入，EF 才知道要刪除哪些東西
            var post = await _dbPosts.GetDbContext().ForumPosts
                .Include(p => p.ForumPostsImages)
                .Include(p => p.ForumPostTagDetails)
                .FirstOrDefaultAsync(p => p.PostId == postId);

            if (post == null) throw new KeyNotFoundException($"找不到 ID 為 {postId} 的貼文");

            if (post.UserId != userId)
                throw new ArgumentException($"您沒有權限刪除他人貼文!");

            // 2. 先刪除圖片關聯
            if (post.ForumPostsImages.Any())
                {
                    _dbPosts.GetDbContext().ForumPostsImages.RemoveRange(post.ForumPostsImages);
                }

            // 3. 刪除標籤明細關聯
            if (post.ForumPostTagDetails.Any())
            {
                _dbPosts.GetDbContext().ForumPostTagDetails.RemoveRange(post.ForumPostTagDetails);
            }

            // 4. 最後刪除貼文主體
            _dbPosts.GetDbContext().ForumPosts.Remove(post);

            // 5. 統一存檔 (這會包在同一個 Transaction 中)
            return await _dbPosts.SaveChangesAsync();
           
        }
    }
}
