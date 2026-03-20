using HomeRepositoryHelper.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalterEFModels.EFModels;
using Microsoft.EntityFrameworkCore;



namespace HomeRepositoryHelper.Repository
{
    public class GenericHomeRepository<Table> : IGenericHomeRepository<Table> where Table : class
    {
        private readonly SalterDbContext? _dbContext = null!;
        private readonly DbSet<Table>? _entity = null!;

        public GenericHomeRepository(SalterDbContext dbContext)
        {
            _dbContext = dbContext;
            _entity = _dbContext.Set<Table>();
        }

        public async Task<Table> AddAsync(Table entity)
        {
            
            _entity.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteByIdAsync(int id)
        {
            // 先找找看有沒有這筆資料
            var entity = await _entity.FindAsync(id);

            if (entity != null)
            {
                _entity.Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Table>> GetAll()
        {
            return await _entity.AsNoTracking().ToListAsync();
        }

        public async Task<Table?> GetByIdAsync(int id)
        {
            // FindAsync 會根據主鍵 (Primary Key) 來搜尋
            return await _entity.FindAsync(id);
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Table entity)
        {
            // 標記這筆資料已被修改
            _entity.Update(entity);
            // 儲存變更
            await _dbContext.SaveChangesAsync();
        }
    }
}
