using HomeServiceHelper.Models.DTO.ViewModels;
using HomeServiceHelper.Models.DTO.ViewModels.Review;
using SalterEFModels.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServiceHelper.IService
{
    public interface IHomService
    {


        // --- 讀取功能 ---
        Task<IEnumerable<HouseDetailViewDTO>> GetAllHousesAsync();

        //  修正：讓分組預覽支援城市篩選，對應前端 getHouseGroups(city) 的需求
        Task<List<CityGroupDTO>> GetCityGroupPreviewsAsync(string? city = null);

        // 搜尋功能（支援關鍵字與地區）
        Task<List<HousePreviewDTO>> SearchHousesAsync(string? city, string? keyword, int? guests);
        public Task<IEnumerable<HousePreviewDTO>> GetSearchHousesAsync(HouseSearchDTO searchDto);

        // 進階搜尋（包含人數、日期、價格區間等 DTO）
        Task<IEnumerable<HouseDetailViewDTO>> SearchHousesAsync(HouseSearchDTO search);

        // 詳情與首頁推薦
        Task<HouseDetailViewDTO> SerchHouseDetailAsync(int roomTypeId, int? currentUserId = null);
        Task<IEnumerable<HouseDetailViewDTO>> GetTopRoomsAsync(int count);

        // 基礎資料撈取
        Task<IEnumerable<string?>> GetAllCityAsync();
        Task<IEnumerable<HomAmenity>> GetAllAmenitiesAsync();
        Task<IEnumerable<BookingListViewModel>> GetMemberBookingsAsync(int userId);

        // 留言CRUD
        Task<bool> AddReviewAsync(ReviewCreateDTO dto);
        Task<bool> UpdateReviewAsync(ReviewUpdateDTO dto);
        Task<bool> DeleteReviewAsync(int reviewId, int memberId);

        // 訂單編號CRUD
        public Task<int> CreateBookingAsync(CreateBookingDTO dto);
        public Task<bool> CancelBookingAsync(int bookingId, int userId);
        public Task<int?> GetAvailableBookingIdAsync(int userId, int roomTypeId);

        // 房屋新增修改刪除
        Task<bool> CreateFullHouseAsync(HouseCreateDTO dto);
        Task<bool> UpdateFullHouseAsync(HouseUpdateDTO dto);




    }
}
