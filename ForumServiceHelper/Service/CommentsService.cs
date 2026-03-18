using ForumRepositoryHelper.IRepository;
using ForumServiceHelper.IService;
using ForumServiceHelper.Models.DTO.CreateModel;
using ForumServiceHelper.Models.DTO.Response;
using Microsoft.EntityFrameworkCore;
using SalterEFModels.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumServiceHelper.Service
{
    public class CommentsService:ICommentsService
    {
        private readonly IGenericSalterRepository<ForumComment> _dbComments;

        public CommentsService(IGenericSalterRepository<ForumComment> dbComments)
        {
            _dbComments = dbComments;
        }

        public async Task<int> CreateCommentAsync(int userId, CommentsCreateModel dto)
        {
            // 1. 基本驗證：內容不能為空
            if (string.IsNullOrWhiteSpace(dto.Content))
            {
                throw new ArgumentException("留言內容不可為空白");
            }

            // 2. 進階驗證：如果有父留言 ID，檢查父留言是否存在
            if (dto.ParentCommentId.HasValue)
            {
                var parentExists = await _dbComments.GetAll()
                    .AnyAsync(c => c.CommentId == dto.ParentCommentId.Value);

                if (!parentExists)
                {
                    throw new ArgumentException("您欲回覆的留言可能已刪除");
                }
            }

            // 3. 建立實體 (Mapping DTO to Entity)
            var newComment = new ForumComment
            {
                PostId = dto.PostId,
                ParentCommentId = dto.ParentCommentId, 
                UserId = userId,
                Content = dto.Content,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // 4. 寫入資料庫
            _dbComments.Add(newComment);
            bool isSaved = await _dbComments.SaveChangesAsync();

            if (!isSaved)
                throw new Exception("資料庫寫入失敗，請稍後再試");

            return newComment.CommentId;
        }
        public async Task<bool> UpdateCommentAsync(int userId, int commentId, CommentsCreateModel dto)
        {
            var existingComment = await _dbComments.GetTableByIDAsync(commentId);

            if (existingComment == null)
                throw new KeyNotFoundException("找不到該則留言，可能已被刪除。");

            // 權限驗證：這是 API 安全的核心！
            // 確保只有留言的原作者 (userId) 才能修改這筆資料
            if (existingComment.UserId != userId)
            {
                throw new ArgumentException("權限不足：您只能修改自己的留言。");
            }

            // 4. 內容檢查
            if (string.IsNullOrWhiteSpace(dto.Content))
            {
                throw new ArgumentException("修改內容不可為空白。");
            }

            // 5. 執行更新
            existingComment.Content = dto.Content;
            existingComment.UpdatedAt = DateTime.Now;

            _dbComments.Update(existingComment);
            bool isSaved = await _dbComments.SaveChangesAsync();

            if (!isSaved)
            {
                throw new Exception("更新失敗：資料庫儲存程序發生異常。");
            }

            return true;
        }
        public async Task<bool> DeleteCommentAsync(int userId, int commentId)
        {
            var existingComment = await _dbComments.GetTableByIDAsync(commentId);

            if (existingComment == null)
            {
                throw new KeyNotFoundException("找不到該則留言，可能先前已被刪除。");
            }

            if (existingComment.UserId != userId)
            {
                throw new ArgumentException("權限不足：您無法刪除不屬於您的留言。");
            }

            _dbComments.Delete(commentId);
            bool isSaved = await _dbComments.SaveChangesAsync();

            if (!isSaved)
            {
                throw new Exception("刪除失敗：伺服器在執行刪除程序時發生異常。");
            }

            return true;
        }
    }
}
