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

public class UserTransactionHistoryRepository : GenericRepository<UserTransactionHistory>, IUserTransactionHistoryRepository
{
    public UserTransactionHistoryRepository() { }
    public UserTransactionHistoryRepository(WorkHiveContext context) => _context = context;

    public async Task<List<UserTransactionHistory>> GetAllUserTransactionHistoryByCustomerWalletId(int customerWalletId)
    {
        return await _context.UserTransactionHistories
            .Where(x => x.CustomerWalletId.Equals(customerWalletId))
            .Include(uth => uth.TransactionHistory)
            .ToListAsync();
    }


}
