using HomeServiceHelper.Models.DTO.ViewModels;
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
        Task<List<HousePreviewDTO>> SearchHousesAsync(string? city, string? keyword , int? guests);

        // 進階搜尋（可能包含人數、日期、價格區間等 DTO）
        Task<IEnumerable<HouseDetailViewDTO>> SearchHousesAsync(HouseSearchDTO search);

        // 詳情與首頁推薦
        Task<HouseDetailViewDTO> SerchHouseDetailAsync(int roomTypeId);
        Task<IEnumerable<HouseDetailViewDTO>> GetTopRoomsAsync(int count);

        // 基礎資料撈取
        Task<IEnumerable<string?>> GetAllCityAsync();
        Task<IEnumerable<HomAmenity>> GetAllAmenitiesAsync();

        // --- 異動功能 (CUD) ---
        Task<bool> AddReviewAsync(ReviewCreateDTO dto);
        Task<bool> CreateFullHouseAsync(HouseCreateDTO dto);
        Task<bool> UpdateFullHouseAsync(HouseUpdateDTO dto);
    }
}
