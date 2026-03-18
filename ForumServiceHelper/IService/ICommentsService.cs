using ForumServiceHelper.Models.DTO.CreateModel;
using ForumServiceHelper.Models.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumServiceHelper.IService
{
    public interface ICommentsService
    {
        public Task<int> CreateCommentAsync(int userId, CommentsCreateModel dto);
        public Task<bool> UpdateCommentAsync(int userId, int commentId, CommentsCreateModel dto);
        public Task<bool> DeleteCommentAsync(int userId, int commentId);

    }
}
