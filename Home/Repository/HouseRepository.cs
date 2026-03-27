using HomeRepositoryHelper.IRepository;
using Microsoft.EntityFrameworkCore;
using SalterEFModels.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeRepositoryHelper.Repository
{
    public class HouseRepository : GenericHomeRepository<HomHouse>, IHouseRepository
    {
        private readonly SalterDbContext? _dbContext;

        public HouseRepository(SalterDbContext? dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<HomRoomType>> SearchAvailableRoomsAsync(
            string city, int? guests, DateOnly? startDate, DateOnly? endDate)
        {
            // 1. 基礎查詢：先 Include 必要的關聯資料
            var query = _dbContext.HomRoomTypes
                .Include(rt => rt.House)
                .Include(rt => rt.HomRoomImages) // 轉 DTO 需要圖片
                .Where(rt => rt.IsActive == true);

            // 2. 地點與人數篩選 (這部分沒問題)
            if (!string.IsNullOrEmpty(city) && city != "全部")
            {
                query = query.Where(rt => rt.House.Citie == city);
            }
            if (guests.HasValue && guests > 0)
            {
                query = query.Where(rt => rt.Capacity >= guests.Value);
            }

            // 3. 日期篩選 (修正 500 錯誤的關鍵)
            if (startDate.HasValue && endDate.HasValue)
            {
                // 【核心修正】：先把 DateOnly 轉回 DateTime，因為資料庫欄位是 DateTime
                // 這樣 EF 就能直接產生簡單的 SQL 比較，不需要在 SQL 裡做轉型
                DateTime startDT = startDate.Value.ToDateTime(TimeOnly.MinValue);
                DateTime endDT = endDate.Value.ToDateTime(TimeOnly.MinValue);

                // 門檻 2：排除已訂房 (比對 DateTime 欄位)
                query = query.Where(rt => !_dbContext.HomBookings.Any(b =>
                    b.RoomTypeId == rt.RoomTypeId &&
                    b.Status != "Cancelled" &&
                    b.CheckInDate < endDT &&   // 使用轉好的 DateTime
                    b.CheckOutDate > startDT   // 使用轉好的 DateTime
                ));

                // 門檻 3：排除不可供應日 (比對 DateOnly 欄位)
                query = query.Where(rt => !_dbContext.HomRoomCalendars.Any(c =>
                    c.RoomTypeId == rt.RoomTypeId &&
                    c.TargetDate >= startDate.Value && // 這裡 TargetDate 本身就是 DateOnly，直接比
                    c.TargetDate < endDate.Value &&
                    c.IsAvailable == false
                ));
            }

            return await query.ToListAsync();
        }
    }
}
