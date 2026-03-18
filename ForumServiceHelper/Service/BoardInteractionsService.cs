using ForumRepositoryHelper.IRepository;
using ForumServiceHelper.IService;
using ForumServiceHelper.Models.DTO.Const;
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
    public class BoardInteractionsService : IBoardInteractionsService
    {
        private readonly IGenericSalterRepository<ForumBoardInteraction> _dbBoardInteract;

        public BoardInteractionsService(IGenericSalterRepository<ForumBoardInteraction> dbBoardInteract)
        {
            _dbBoardInteract = dbBoardInteract;
        }
        public async Task<BoardInteractionResponseModel> ProcessInteractionAsync(int userId, BoardInteractionCreateModel dto)
        {
            // 1. 檢查資料庫是否已存在該用戶對該貼文的特定類型互動，如果有的話撈出那一筆資料
            var existing = await _dbBoardInteract.GetAll()
                .FirstOrDefaultAsync(x => x.BoardId == dto.BoardId &&
                x.UserId == userId &&
                x.Type == dto.Type);

            switch (dto.Type.ToUpper())
            {
                case BoardInteractionTypes.Follow:
                    return await HandleToggleInteraction(userId, dto, existing);

                case BoardInteractionTypes.View:
                    return await HandleViewInteraction(userId, dto);

                default:
                    return new BoardInteractionResponseModel
                    {
                        Success = false,
                        Message = "不支援的互動類型",
                        CurrentStatus = "ERROR"
                    };
            }
        }


        public async Task<BoardInteractionResponseModel> HandleToggleInteraction(int userId, BoardInteractionCreateModel dto, ForumBoardInteraction? existing)
        {
            if (existing != null)
            {
                // 偶數次：執行刪除
                _dbBoardInteract.Delete(existing.InteractionId);
                bool isSaved = await _dbBoardInteract.SaveChangesAsync();
                if (!isSaved) throw new Exception("資料庫儲存失敗!");

                return new BoardInteractionResponseModel
                {
                    Success = isSaved,
                    Message = $"{dto.Type} 已移除",
                    CurrentStatus = "UNFOLLOWED" // 讓前端 Angular 知道按鈕要變回灰色
                };
            }

            // 奇數次：執行新增
            await CreateNewInteraction(userId, dto);
            return new BoardInteractionResponseModel
            {
                Success = true,
                Message = $"{dto.Type} 成功",
                CurrentStatus = "FOLLOWED" // 讓前端 Angular 知道按鈕要變亮
            };
        }

        public async Task<BoardInteractionResponseModel> HandleViewInteraction(int userId, BoardInteractionCreateModel dto)
        {
            await CreateNewInteraction(userId, dto);
            return new BoardInteractionResponseModel { Success = true, Message = "瀏覽紀錄已更新" };
        }
        public async Task CreateNewInteraction(int userId, BoardInteractionCreateModel dto)
        {
            var entity = new ForumBoardInteraction
            {
                BoardId = dto.BoardId,
                UserId = userId,
                Type = dto.Type.ToUpper(),
                CreatedAt = DateTime.Now
            };

            _dbBoardInteract.Add(entity);
            await _dbBoardInteract.SaveChangesAsync();
        }

    }
}
