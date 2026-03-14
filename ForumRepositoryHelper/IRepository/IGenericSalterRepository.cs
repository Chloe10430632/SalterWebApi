using SalterEFModels.EFModels;

namespace ForumRepositoryHelper.IRepository
{
    public interface IGenericSalterRepository<Table> where Table : class
    {
        SalterDbContext GetDbContext();

        IQueryable<Table> GetAll();

        Task<IEnumerable<Table>> GetAllAsync();

        ValueTask<Table?> GetTableByIDAsync<PrimaryKeyType>(PrimaryKeyType id);

        void Add(Table entity);

        // 刪除資料 (建議先非同步取得實體再刪除，或透過 ID 進行非同步查找)
         void Delete<PrimaryKeyType>(PrimaryKeyType id);

        void Update(Table entity);

        Task<bool> SaveChangesAsync();
    }
}
