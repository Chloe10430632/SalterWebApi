using SalterEFModels.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserRepositoryHelper.IRepository
{
    internal interface IGenericUserRepository<Table> where Table : class
    {
        Task<SalterDbContext> GetDbContextAsync();

        Task<IEnumerable<Table>> GetAllAsync();

        Task<IEnumerable<Table>> GetByUserId(int id);

        Task CreateAsync(Table entity);

        Task UpdateAsync(Table entity);

        Task DeleteAsync(Table entity);

        Task<bool> SaveChangesAsync();


    }
}
