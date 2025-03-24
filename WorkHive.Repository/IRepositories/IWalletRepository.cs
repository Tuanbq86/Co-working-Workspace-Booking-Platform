using WorkHive.Data.Base;
using WorkHive.Data.Models;

namespace WorkHive.Repositories.IRepositories;

public interface IWalletRepository : IGenericRepository<Wallet>
{
    public Task<Wallet?> GetOwnerWalletByIdAsync(int id); 
    public Task<List<Wallet>> GetAllWalletOwnersAsync();
    //public Task<Wallet?> GetByOwnerIdAsync(int ownerId);

    //public Task UpdateWalletAsync(Wallet wallet);
}
