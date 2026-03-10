using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeRepositoryHelper.IRepository
{
    public interface IGenericHomeRepository<Table> where Table : class
    {
        public Task<IEnumerable<Table>> GetAllHouse();
        public  Task<Table?> GetByIdAsync(int id);
        public Task<Table> Add(Table entity);
        public Task DeleteByIdAsync(int id);
        public  Task UpdateAsync(Table entity);
        public  Task SaveAsync();
    }
}
