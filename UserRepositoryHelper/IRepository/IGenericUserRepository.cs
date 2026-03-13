using SalterEFModels.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserRepositoryHelper.IRepository
{
    public interface IGenericUserRepository<Table> where Table : class
    {
        SalterDbContext GetDbContext();

        Task<IEnumerable<Table>> GetAllAsync();

        Task<Table?> GetByIdAsync(int id);

        Task CreateAsync(Table entity);

        void Update(Table entity);

        void Delete(Table entity);

        Task<bool> SaveChangesAsync();


    }
}
