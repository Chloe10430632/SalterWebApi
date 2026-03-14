using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ForumRepositoryHelper.IRepository;
using ForumServiceHelper.IService;
using ForumServiceHelper.Models.DTO.Const;
using ForumServiceHelper.Models.DTO.CreateModel;
using ForumServiceHelper.Models.DTO.QueryModel;
using ForumServiceHelper.Models.DTO.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SalterEFModels.EFModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<PostListViewModel>> GetAllPostsAsync(PostsQueryModel query)
        {
            var posts = _dbPosts.GetAll().Where(p => p.IsPosted && p.Status == PostCreateStatusTypes.Normal);

            if (query.BoardId.HasValue) 
                posts = posts.Where(p => p.BoardId == query.BoardId);

            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                var kw = query.Keyword.Trim();
                posts = posts.Where(p => p.Content.Contains(kw) || p.Board.Title.Contains(kw));
            }

            //處理 Follow 邏輯(使用 Join 效率更高)
            if (query.SortBy == SortTypes.Follow && query.UserId.HasValue)
            {
                var followedBoardIds = _dbPosts.GetDbContext().ForumBoardInteractions
                    .Where(bi => bi.UserId == query.UserId && bi.Type == BoardInteractionTypes.Follow)
                    .Select(bi => bi.BoardId);
                posts = posts.Where(p => followedBoardIds.Contains(p.BoardId));
            }

            var postListQuery = posts.Select(p => new PostListViewModel
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

        public async Task<int> CheckAndCreateAsync(PostCreateModel data, int? postId = null) //AddOrUpdate
        {
            if (data == null) return -1;

            ForumPost? newPost;

            if (postId.HasValue && postId.Value > 0)
            {
                // --- 更新邏輯 ---
                // 抓出舊貼文，並包含圖片與標籤明細（為了後續清理）
                newPost = await _dbPosts.GetDbContext().ForumPosts
                    .Include(p => p.ForumPostsImages)
                    .Include(p => p.ForumPostTagDetails)
                    .FirstOrDefaultAsync(p => p.PostId == postId.Value);

                if (newPost == null) return -1; // 找不到原本的貼文

                // 更新內容
                newPost.BoardId = data.BoardId;
                newPost.Content = data.Content;
                newPost.LocationId = data.LocationId;
                newPost.Status = data.isPosted ? PostCreateStatusTypes.Normal : PostCreateStatusTypes.Hide;
                newPost.IsPosted = data.isPosted;
                newPost.UpdatedAt = DateTime.Now;

              
            }
            else
            {
                // --- 新增邏輯 ---
                newPost = new ForumPost
                {
                    UserId = data.UserId,
                    BoardId = data.BoardId,
                    Content = data.Content,
                    LocationId = data.LocationId,
                    Status = data.isPosted ? PostCreateStatusTypes.Normal : PostCreateStatusTypes.Hide,
                    IsPinned = false,
                    IsPosted = data.isPosted,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _dbPosts.Add(newPost);
            }

            // 2. 處理圖片 (Cloudinary)
            if (data.Images is { Count: > 0 })
            {
                var account = new Account(_cloudinaryName, _cloudinaryApiKey, _cloudinaryApiSecret);
                var cloudinary = new Cloudinary(account);

                int fileIndex = 1;
                foreach (var file in data.Images)
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
                        var postImage = new ForumPostsImage
                        {
                            // 這裡不需要寫 PostId，EF 會在 SaveChanges 時自動關聯 newPost
                            Post = newPost,
                            ImageUrl = uploadResult.SecureUrl.ToString(),
                            SortIndex = fileIndex++
                        };
                        _dbPosts.GetDbContext().ForumPostsImages.Add(postImage);
                    }else
                    {
                        return -1; // 上傳失敗
                    }
                }
            }

            // 3. 處理標籤 (效能關鍵：減少資料庫查詢次數)
            if (data.Tags is { Count: > 0 })
            {
                //1.先將使用者上傳的標籤集合做整理，先把空白與重複項刪除(記憶體)
                var tagNames = data.Tags.Select(t => t.TagName.Trim()).Distinct().ToList();

                // 2.直接拿使用者上傳的整包標籤去資料庫做查詢比對(記憶體)
                var existingTags = _dbPosts.GetDbContext().ForumTags
                    .Where(t => tagNames.Contains(t.TagName))
                    .ToList();

                foreach (var tagName in tagNames)
                {
                    var targetTag = existingTags.FirstOrDefault(t => t.TagName == tagName);

                    // 如果標籤不存在，建立新的
                    if (targetTag == null)
                    {
                        targetTag = new ForumTag { TagName = tagName };
                        _dbPosts.GetDbContext().ForumTags.Add(targetTag);
                    }

                    // 建立關聯 (Detail 表)
                    var detail = new ForumPostTagDetail
                    {
                        Post = newPost, // 直接關聯物件，EF 會處理 ID
                        Tag = targetTag  // 直接關聯物件
                    };
                    _dbPosts.GetDbContext().ForumPostTagDetails.Add(detail);
                }
            }

            // 4. 最後一次 SaveChanges，完成所有動作 (包含 Transaction)
            // 即使沒有圖片或標籤，這行也會執行，且不會報錯
            try
            {
                await _dbPosts.SaveChangesAsync();
                return newPost.PostId;
            }
            catch (Exception)
            {
                // TODO 記錄錯誤...
                return -1;
            }































            //List<string> finalImageUrls = new List<string>();
            //ForumPost newPost;
            //if (data != null)
            //{
            //    if (data.isPosted)
            //    {
            //        //正式發布
            //        newPost = new ForumPost
            //        {
            //            UserId = data.UserId,
            //            BoardId = data.BoardId,
            //            Content = data.Content,
            //            LocationId = data.LocationId,
            //            Status = PostCreateStatusTypes.Normal,
            //            IsPinned = false,
            //            IsPosted = true,
            //            CreatedAt = DateTime.Now,
            //            UpdatedAt = DateTime.Now
            //        };
            //        _dbPosts.Add(newPost);
            //    }
            //    else
            //    {
            //        //建立貼文草稿
            //         newPost = new ForumPost
            //        {
            //            UserId = data.UserId,
            //            BoardId = data.BoardId,
            //            Content = data.Content,
            //            LocationId = data.LocationId,
            //            Status = PostCreateStatusTypes.Hide,
            //            IsPinned = false,
            //            IsPosted = false,
            //            CreatedAt = DateTime.Now,
            //            UpdatedAt = DateTime.Now
            //        };
            //        _dbPosts.Add(newPost);
            //    }
            //    bool isPostSaved = await _dbPosts.SaveChangesAsync();

            //    if (isPostSaved)
            //    {
            //        // 2. 處理圖片
            //        if (data.Images !=  null && data.Images.Count>0)
            //        {
            //            int fileIndex = 1;
            //            foreach (var file in data.Images)
            //            {
            //                if (file.Length > 0)
            //                {
            //                    // 建立 Cloudinary 帳戶實例 (建議從設定檔讀取或 DI 注入)
            //                    var account = new Account(_cloudinaryName, _cloudinaryApiKey, _cloudinaryApiSecret);
            //                    var cloudinary = new Cloudinary(account);

            //                    // 使用資料流讀取上傳圖片
            //                    using var stream = file.OpenReadStream();
            //                    var uploadParams = new ImageUploadParams()
            //                    {
            //                        File = new FileDescription(file.FileName, stream),
            //                        Folder = "Salter/Forum",
            //                        PublicId = $"{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString().Substring(0, 4)}",
            //                        // 可加入圖片轉換設定 (例如限定大小)
            //                        Transformation = new Transformation().Quality("auto").FetchFormat("auto")
            //                    };

            //                    // 執行非同步上傳
            //                    var uploadResult = cloudinary.Upload(uploadParams);
            //                    if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            //                    {
            //                        // 取得 Cloudinary 回傳的 SecureUrl (https)
            //                        string uploadedUrl = uploadResult.SecureUrl.ToString();
            //                        finalImageUrls.Add(uploadedUrl);
            //                        // 將相對路徑存入資料庫 (供前端存取使用)
            //                        var postImage = new ForumPostsImage
            //                        {
            //                            PostId = newPost.PostId,
            //                            ImageUrl = uploadedUrl,
            //                            SortIndex = fileIndex
            //                        };
            //                        _dbPosts.GetDbContext().ForumPostsImages.Add(postImage);
            //                    }
            //                    else
            //                    {
            //                        return -1;
            //                    }
            //                }
            //                fileIndex++;
            //            }
            //        }

            //        // 3. 處理標籤關聯 (有標籤才做動作)
            //        if (data.Tags!=null && data.Tags.Count>0)
            //        {
            //            foreach (var tag in data.Tags)
            //            {
            //                // 找出標籤文字是否已存在?不存在就新增標籤
            //                tag.TagName = tag.TagName.Trim();

            //                //Tag不存在
            //                if (!_dbPosts.GetDbContext().ForumTags.Any(t => t.TagName == tag.TagName))
            //                {
            //                    //不存在，新增貼文標籤
            //                    var newTag = new ForumTag
            //                    {
            //                        TagName = tag.TagName
            //                    };
            //                    _dbPosts.GetDbContext().ForumTags.Add(newTag);
            //                    bool isTagSaved = await _dbPosts.SaveChangesAsync();
            //                    if (isTagSaved)
            //                    {
            //                        //已存在，直接新增貼文標籤明細表
            //                        var newPostnewTagsDetail = new ForumPostTagDetail
            //                        {
            //                            PostId = newPost.PostId,
            //                            TagId = newTag.TagId
            //                        };
            //                        _dbPosts.GetDbContext().ForumPostTagDetails.Add(newPostnewTagsDetail);
            //                    }
            //                }
            //                else
            //                {
            //                    //Tag已存在，直接新增貼文標籤明細表
            //                    var newPostTagsDetail = new ForumPostTagDetail
            //                    {
            //                        PostId = newPost.PostId,
            //                        TagId = tag.TagId
            //                    };
            //                    _dbPosts.GetDbContext().ForumPostTagDetails.Add(newPostTagsDetail);
            //                }

            //            }
            //        }

            //    }

            //            bool allSaved = await _dbPosts.SaveChangesAsync();
            //            if (allSaved)
            //                return newPost.PostId; // 回傳 ID 讓前端可以跳轉
            //            else
            //                return -1;
            //}
            //return -1; //失敗了就回傳-1，成功了回傳˙貼文Id
        }

        public async Task<bool> CheckAndDeleteAsync(int postId)
        {
            // 1. 抓出貼文主體，同時 Include 所有關聯表
            // 這是關鍵：必須把關聯資料一併載入，EF 才知道要刪除哪些東西
            var post = await _dbPosts.GetDbContext().ForumPosts
                .Include(p => p.ForumPostsImages)
                .Include(p => p.ForumPostTagDetails)
                .FirstOrDefaultAsync(p => p.PostId == postId);

            if (post == null) return false;

            try
            {
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
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
