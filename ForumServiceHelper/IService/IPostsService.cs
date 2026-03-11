using ForumServiceHelper.Models.DTO.ViewModel;
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
       public Task<IList<PostsViewModel>> GetAllPostsAsync(int postId);

    }
}
