using ForumServiceHelper.Models.DTO.CreateModel;
using ForumServiceHelper.Models.DTO.Response;
using SalterEFModels.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumServiceHelper.IService
{
    public interface IPostInteractionsService
    {
        public Task<PostInteractionResponseModel> ProcessInteractionAsync(int userId, PostInteractionCreateModel dto);

        public Task<PostInteractionResponseModel> HandleToggleInteraction(int userId, PostInteractionCreateModel dto, ForumPostInteraction? existing);
        public Task<PostInteractionResponseModel> HandleShareInteraction(int userId, PostInteractionCreateModel dto, ForumPostInteraction? existing);
        public Task<PostInteractionResponseModel> HandleReportInteraction(int userId, PostInteractionCreateModel dto, ForumPostInteraction? existing);
        public Task<PostInteractionResponseModel> HandleViewInteraction(int userId, PostInteractionCreateModel dto);
        public Task CreateNewInteraction(int userId, PostInteractionCreateModel dto, string status);


    }
}
