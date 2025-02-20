namespace WorkHive.Data.Base;
public interface IGenericRepository<T> where T : class
{
    public void PrepareCreate(T entity);
    public void PrepareUpdate(T entity);
    public void PrepareRemove(T entity);
    public List<T> GetAll();
    public Task<List<T>> GetAllAsync();
    public void Create(T entity);
    public Task<int> CreateAsync(T entity);
    public void Update(T entity);
    public Task<int> UpdateAsync(T entity);
    public bool Remove(T entity);
    public Task<bool> RemoveAsync(T entity);
    public T GetById(int id);
    public Task<T> GetByIdAsync(int id);
    public T GetById(string code);
    public Task<T> GetByIdAsync(string code);
    public T GetById(Guid code);
    public Task<T> GetByIdAsync(Guid code);
}
