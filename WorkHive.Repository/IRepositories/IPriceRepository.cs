using WorkHive.Data.Base;
using WorkHive.Data.Models;

namespace WorkHive.Repositories.IRepositories;

public interface IPriceRepository : IGenericRepository<Price>
{
    public Task CreatePricesAsync(List<Price> prices);
    public Task<List<Price>> GetPricesByWorkspaceIdAsync(int workspaceId);
}
