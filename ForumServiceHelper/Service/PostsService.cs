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

        public async Task<IList<PostsViewModel>> GetAllPostsAsync()
        {
            // 1. 從 Repository 取得 EF Models 貼文的所有關聯表
            var posts = _dbPosts.GetDbContext().ForumPosts
                .Include(p => p.User)
                .Include(p => p.Board)
                .Include(p => p.ForumPostsImages)
                .Include(p => p.ForumPostInteractions)
                .Include(p => p.ForumComments);

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

                //處理父子留言結構
                LatestComments = p.ForumComments
                 .OrderByDescending(c => c.CreatedAt)
                 //.Take(1) // 根據 UI 需求取前兩則
                 .Select(c => new CommentPreviewDto
                 {
                     UserName = c.User.UserName,
                     Content = c.Content,
                     AvatarUrl = c.User.ProfilePicture,
                     CreatedAt = c.CreatedAt
                 }).ToList(),


                PostTags = _dbPosts.GetDbContext().ForumPostTagDetails
                 .Include(pt => pt.Tag)
                 .Where(pt => pt.PostId == p.PostId)
                 .Select(pt =>  pt.Tag.TagName )
                 .ToList()
            });

            return await postDetails.ToListAsync();
        }
    }
}
