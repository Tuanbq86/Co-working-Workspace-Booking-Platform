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

public class TransactionHistoryRepository : GenericRepository<TransactionHistory>, ITransactionHistoryRepository
{
    public TransactionHistoryRepository() { }
    public TransactionHistoryRepository(WorkHiveContext context) => _context = context;

    public async Task<List<TransactionHistory>> GetTransactionsByOwnerIdAsync(int ownerId)
    {
        return await _context.TransactionHistories
            .Where(th => th.OwnerTransactionHistories.Any(oth => oth.OwnerWallet.OwnerId == ownerId))
            .Include(th => th.OwnerTransactionHistories)
                .ThenInclude(oth => oth.OwnerWallet)
            .ToListAsync();
    }


}
