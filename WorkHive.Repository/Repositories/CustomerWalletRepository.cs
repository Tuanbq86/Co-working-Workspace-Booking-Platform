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

public class CustomerWalletRepository : GenericRepository<CustomerWallet>, ICustomerWalletRepository
{
    public CustomerWalletRepository() { }
    public CustomerWalletRepository(WorkHiveContext context) => _context = context;

    public async Task<CustomerWallet> GetCustomerWalletByUserId(int UserId)
    {
        return await _context.CustomerWallets.Where(cw => cw.UserId.Equals(UserId))
            .Include(cw => cw.Wallet).FirstOrDefaultAsync();
    }
}
