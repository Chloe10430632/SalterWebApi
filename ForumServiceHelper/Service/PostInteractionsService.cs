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
    public class PostInteractionsService: IPostInteractionsService
    {
        private readonly IGenericSalterRepository<ForumPostInteraction> _dbPostInteract;

        public PostInteractionsService(IGenericSalterRepository<ForumPostInteraction> dbPostInteract)
        {
            _dbPostInteract = dbPostInteract;
        }

        public async Task<PostInteractionResponseModel> ProcessInteractionAsync(int userId, PostInteractionCreateModel dto)
        {
            // 1. 檢查資料庫是否已存在該用戶對該貼文的特定類型互動，如果有的話撈出那一筆資料
            var existing = await _dbPostInteract.GetAll()
                .FirstOrDefaultAsync(x => x.PostId == dto.PostId &&
                                          x.UserId == userId &&
                                          x.Type == dto.Type);

            // 2. 根據互動類型執行對應的商業邏輯
            switch (dto.Type.ToUpper())
            {
                case PostInteractionType.Like:
                case PostInteractionType.Collect:
                    return await HandleToggleInteraction(userId, dto, existing);

                case PostInteractionType.Share:
                    return await HandleShareInteraction(userId, dto, existing);

                case PostInteractionType.Report:
                    return await HandleReportInteraction(userId, dto, existing);

                case PostInteractionType.View:
                    return await HandleViewInteraction(userId, dto);

                default:
                    return new PostInteractionResponseModel
                    {
                        Success = false,
                        Message = "不支援的互動類型",
                        CurrentStatus = "ERROR"
                    };
            }
        }


        #region 私有邏輯處理方法 (Private Logic Handlers)

        // 處理【按讚 & 收藏】：奇數新增，偶數移除
        public async Task<PostInteractionResponseModel> HandleToggleInteraction(
            int userId, PostInteractionCreateModel dto, ForumPostInteraction? existing)
        {
            if (existing != null)
            {
                // 偶數次：執行刪除
                _dbPostInteract.Delete(existing.InteractionId);
                bool isSaved = await _dbPostInteract.SaveChangesAsync();
                if (!isSaved) throw new Exception("資料庫儲存失敗!");

                return new PostInteractionResponseModel
                {
                    Success = isSaved,
                    Message = $"{dto.Type} 已移除",
                    CurrentStatus = "INACTIVE" // 讓前端 Angular 知道按鈕要變回灰色
                };
            }

            // 奇數次：執行新增
            if (userId > 0)
            {
                await CreateNewInteraction(userId, dto, "ACTIVE");
            }
            return new PostInteractionResponseModel
            {
                Success = true,
                Message = $"{dto.Type} 成功",
                CurrentStatus = "ACTIVE" // 讓前端 Angular 知道按鈕要變亮
            };
        }

        // 處理【分享】：若已存在則不新增
        public async Task<PostInteractionResponseModel> HandleShareInteraction(
            int userId, PostInteractionCreateModel dto, ForumPostInteraction? existing)
        {
            if (existing != null)
            {
                return new PostInteractionResponseModel { Success = true, Message = "此貼文先前已分享過" };
            }
            if (userId > 0)
                await CreateNewInteraction(userId, dto, "ACTIVE");
            return new PostInteractionResponseModel { Success = true, Message = "分享紀錄已儲存" };
        }

        // 處理【檢舉】：若已存在則禁止重複提交
        public async Task<PostInteractionResponseModel> HandleReportInteraction(
            int userId, PostInteractionCreateModel dto, ForumPostInteraction? existing)
        {
            if (userId == 0) throw new UnauthorizedAccessException("檢舉必須要登入喔!");

            if (existing != null)
            {
                throw new ArgumentException("您已對這篇文章提交過檢舉，請靜候後台審核");
            }

            if (string.IsNullOrWhiteSpace(dto.ReportReason))
            {
                throw new ArgumentException ("提交檢舉必須附上原因" );
            }

            await CreateNewInteraction(userId, dto, "PENDING"); // 預設狀態為 PENDING
            return new PostInteractionResponseModel { Success = true, Message = "檢舉已送出" };
        }

        // 處理【瀏覽】：單純新增紀錄
        public async Task<PostInteractionResponseModel> HandleViewInteraction(int userId, PostInteractionCreateModel dto)
        {
            if (userId > 0)
                await CreateNewInteraction(userId, dto, "ACTIVE");
            return new PostInteractionResponseModel { Success = true, Message = "瀏覽紀錄已更新" };
        }

        // 封裝實體建立邏輯
        public async Task CreateNewInteraction(int userId, PostInteractionCreateModel dto, string status)
        {
            var entity = new ForumPostInteraction
            {
                PostId = dto.PostId,
                UserId = userId,
                Type = dto.Type.ToUpper(),
                ReportReason = dto.ReportReason,
                Status = status,
                CreatedAt = DateTime.Now
            };

            _dbPostInteract.Add(entity);
            await _dbPostInteract.SaveChangesAsync();
        }

        #endregion

    }
}
