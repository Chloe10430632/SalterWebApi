using HomeServiceHelper.Models.DTO.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeServiceHelper.IService
{
    public interface IHomService
    {
        Task<IEnumerable<HouseDetailViewDTO>> GetAllHousesAsync();
        Task<IEnumerable<HouseDetailViewDTO>> SearchHousesAsync(HouseSearchDTO search);
        Task <HouseDetailViewDTO> SerchHouseDetailAsync(int roomTypeId);
    }
}
