using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Base;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;

namespace WorkHive.Repositories.Repositories;

public class WalletRepository : GenericRepository<Wallet>, IWalletRepository
{
    public WalletRepository() { }
    public WalletRepository(WorkHiveContext context) => _context = context;

    public async Task<Wallet?> GetOwnerWalletByIdAsync(int ownerId)
    {
        return await _context.Wallets
        .Include(w => w.OwnerWallets)
            .ThenInclude(ow => ow.Owner)
        .FirstOrDefaultAsync(w => w.OwnerWallets.Any(ow => ow.OwnerId == ownerId));
    }

    public async Task<List<Wallet>> GetAllWalletOwnersAsync()
    {
        return await _context.Wallets
        .Where(w => w.OwnerWallets.Any()) 
        .Include(w => w.OwnerWallets)
        .ThenInclude(ow => ow.Owner)
        .ToListAsync();
    }

    //public async Task<Wallet?> GetByOwnerIdAsync(int ownerId)
    //{
    //    return await _context.Set<Wallet>()
    //        .Include(w => w.OwnerWallets)
    //        .FirstOrDefaultAsync(w => w.OwnerWallets.Any(ow => ow.OwnerId == ownerId));
    //}

    //public async Task UpdateWalletAsync(Wallet wallet)
    //{
    //    _context.Set<Wallet>().Update(wallet);
    //    await _context.SaveChangesAsync();
    //}
}


