using ForumRepositoryHelper.IRepository;
using ForumServiceHelper.IService;
using ForumServiceHelper.Models.DTO.Const;
using ForumServiceHelper.Models.DTO.ViewModel;
using Microsoft.EntityFrameworkCore;
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

        public PostsService(IGenericSalterRepository<ForumPost> dbPosts)
        {
            _dbPosts = dbPosts;
        }

        public async Task<IList<PostsViewModel>> GetAllPostsAsync(int? postId = null, string? keyword = null, string sortBy = SortTypes.Popular, int? userId = null)
        {
            //1.強制指定變數類型為 IQueryable < ForumPost >，這樣後續的 Where 才能順利對接
            IQueryable<ForumPost> posts = _dbPosts.GetDbContext().ForumPosts
                .Include(p => p.User)
                .Include(p => p.Board)
                .Include(p => p.ForumPostsImages)
                .Include(p => p.ForumPostInteractions)
                .Include(p => p.ForumComments);

            //現在 posts 是 IQueryable，你可以自由地疊加過濾條件
            if (postId.HasValue)
            {
                posts = posts.Where(p => p.PostId == postId.Value);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
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


            if (sortBy != null)
            {
                if (sortBy == SortTypes.Popular || sortBy == SortTypes.Follow)
                {
                    postDetails = postDetails.OrderByDescending(pd=>pd.ViewCount);
                }

                if(sortBy == SortTypes.New)
                {
                    postDetails = postDetails.OrderByDescending(pd => pd.CreatedAt);
                }
            }
            return await postDetails.ToListAsync();
        }

       

    }
}
