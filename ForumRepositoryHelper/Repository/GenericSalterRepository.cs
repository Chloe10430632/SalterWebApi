using Microsoft.EntityFrameworkCore;
using ForumRepositoryHelper.IRepository;
using SalterEFModels.EFModels;

namespace ForumRepositoryHelper.Repository
{
    public class GenericSalterRepository<Table> : IGenericSalterRepository<Table> where Table : class
    {
        private readonly SalterDbContext _dbContext;
        private readonly DbSet<Table> _entity;

        public GenericSalterRepository(SalterDbContext dbContext)
        {
            _dbContext = dbContext;
            _entity = dbContext.Set<Table>();
        }
        public SalterDbContext GetDbContext()
        {
            return _dbContext;
        }

        public async Task<IEnumerable<Table>> GetAllAsync()
        {
            return  await _entity.AsNoTracking().ToListAsync();
        }

        public async ValueTask<Table?> GetTableByIDAsync<PrimaryKeyType>(PrimaryKeyType id)
        {
            // 使用 FindAsync(內建快取機制) 進行非同步主鍵查詢
            // (使用 ValueTask 在緩存命中時效能較佳)
            return await _entity.FindAsync(id);
        }

        public void Add(Table entity)
        {
            _entity.Add(entity);
        }

        public async Task DeleteAsync<PrimaryKeyType>(PrimaryKeyType id)
        {
            // 先非同步找到該筆資料
            var target = await _entity.FindAsync(id);
            if (target != null)
            {
                _entity.Remove(target);
            }
        }
        public void Update(Table entity)
        {
            _entity.Update(entity);
        }
        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                // 資料庫真正寫入之後會回傳筆數
                var result = await _dbContext.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
