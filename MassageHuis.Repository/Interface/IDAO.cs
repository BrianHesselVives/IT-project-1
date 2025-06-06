namespace MassageHuis.Repositories.Interfaces
{
    public interface IDAO<T> where T : class
    {
        Task<IEnumerable<T>?> GetAllAsync();
        Task AddAsync(T entity);
        Task DeleteAsync(T entity);

        Task UpdateAsync(T entity);
        Task<T?> FindByIdAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task DeleteRangeAsync(IEnumerable<T> entities);
    }
}
