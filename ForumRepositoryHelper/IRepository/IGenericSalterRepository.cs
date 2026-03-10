using SalterEFModels.EFModels;

namespace ForumRepositoryHelper.IRepository
{
    public interface IGenericSalterRepository<Table> where Table : class
    {
        SalterDbContext GetDbContext();

        Task<IEnumerable<Table>> GetAllAsync();

        // 根據主鍵取得單筆資料 (使用 ValueTask 在緩存命中時效能較佳)
        ValueTask<Table?> GetTableByIDAsync<PrimaryKeyType>(PrimaryKeyType id);

        void Add(Table entity);

        // 刪除資料 (建議先非同步取得實體再刪除，或透過 ID 進行非同步查找)
        Task DeleteAsync<PrimaryKeyType>(PrimaryKeyType id);

        void Update(Table entity);

        Task<bool> SaveChangesAsync();
    }
}
