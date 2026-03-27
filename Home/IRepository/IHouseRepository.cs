using SalterEFModels.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeRepositoryHelper.IRepository
{
    public interface IHouseRepository : IGenericHomeRepository<HomHouse>
    {
        Task<IEnumerable<HomRoomType>> SearchAvailableRoomsAsync(string city, int? guests, DateOnly? startDate, DateOnly? endDate);
    }
}
