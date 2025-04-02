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

public class OwnerTransactionHistoryRepository : GenericRepository<OwnerTransactionHistory>, IOwnerTransactionHistoryRepository
{
    public OwnerTransactionHistoryRepository() { }
    public OwnerTransactionHistoryRepository(WorkHiveContext context) => _context = context;

    public async Task<List<OwnerTransactionHistory>> GetByOwnerIdAsync(int ownerId)
    {
        return await _context.OwnerTransactionHistories
            .Where(r => r.OwnerWallet.OwnerId == ownerId)
            .ToListAsync();
    }

    public async Task<OwnerTransactionHistory?> GetLatestTransactionByOwnerIdAsync(int ownerId)
    {
        return await _context.OwnerTransactionHistories
            .Where(oth => oth.OwnerWallet.OwnerId == ownerId)
            .OrderByDescending(oth => oth.TransactionHistory.CreatedAt)
            .FirstOrDefaultAsync();
    }

}
