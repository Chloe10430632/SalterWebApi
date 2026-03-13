using Microsoft.EntityFrameworkCore;
using SalterEFModels.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserRepositoryHelper.IRepository;

namespace UserRepositoryHelper.Repository
{
    public class GenericUserRepository<Table> : IGenericUserRepository<Table> where Table : class
    {
        private readonly SalterDbContext _dbContext;
        private readonly DbSet<Table> _entity;

        public GenericUserRepository(SalterDbContext dbContext)
        {
            _dbContext = dbContext;
            _entity = _dbContext.Set<Table>();
        }

        public SalterDbContext GetDbContext()
        {
            return _dbContext;
        }

        public async Task<IEnumerable<Table>> GetAllAsync()
        {
            return await _entity.ToListAsync();
        }

        public async Task<Table?> GetByIdAsync(int id)
        {
            return await _entity.FindAsync(id);
        }


        public async Task CreateAsync(Table entity)
        {
            await _entity.AddAsync(entity);
        }

        public void Update(Table entity)
        {
            _entity.Update(entity);
        }

        public void Delete(Table entity)
        {
            _entity.Remove(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch(Exception)
            {
                return false;
            }
            
        }

    }
}
