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

public class OwnerWalletRepository : GenericRepository<OwnerWallet>, IOwnerWalletRepository
{
    public OwnerWalletRepository() { }
    public OwnerWalletRepository(WorkHiveContext context) => _context = context;

    public async Task<OwnerWallet> GetOwnerWalletByOwnerIdForBooking(int OwnerId)
    {
        return await _context.OwnerWallets.Where(ow => ow.OwnerId.Equals(OwnerId))
            .Include(ow => ow.Wallet)
            .FirstOrDefaultAsync();
    }
}
