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
    public interface IBoardInteractionsService
    {
        public Task<BoardInteractionResponseModel> ProcessInteractionAsync(int userId, BoardInteractionCreateModel dto);
        public Task<BoardInteractionResponseModel> HandleToggleInteraction(int userId, BoardInteractionCreateModel dto, ForumBoardInteraction? existing);
        public Task<BoardInteractionResponseModel> HandleViewInteraction(int userId, BoardInteractionCreateModel dto);
        public Task CreateNewInteraction(int userId, BoardInteractionCreateModel dto);

    }
}
