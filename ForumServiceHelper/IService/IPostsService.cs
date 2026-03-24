using ForumServiceHelper.Models.DTO.CreateModel;
using ForumServiceHelper.Models.DTO.QueryModel;
using ForumServiceHelper.Models.DTO.ViewModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumServiceHelper.IService
{
    public interface IPostsService
    {
       public Task<IEnumerable<PostListViewModel>> GetAllPostsAsync(int userId,PostsQueryModel query);

        public Task<PostDetailViewModel?> GetPostDetailAsync(int postId);

        public Task<List<string>> UploadToCloudinaryAsync(List<IFormFile> images);

        public Task<int> CheckAndCreateAsync(int userId,PostCreateModel data, int? postId = null);

        public Task<bool> CheckAndDeleteAsync(int userId, int postId);

    }
}
