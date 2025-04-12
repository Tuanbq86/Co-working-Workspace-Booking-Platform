using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkHive.Data.Base;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;

namespace WorkHive.Repositories.Repositories;

public class CustomerWithdrawalRequestRepository : GenericRepository<CustomerWithdrawalRequest>, ICustomerWithdrawalRequestRepository
{
    public CustomerWithdrawalRequestRepository() { }
    public CustomerWithdrawalRequestRepository(WorkHiveContext context) => _context = context;
    public async Task<List<CustomerWithdrawalRequest>> GetByCustomerIdAsync(int customerId)
    {
        return await _context.CustomerWithdrawalRequests
            .Where(r => r.UserId == customerId)
            .ToListAsync();
    }
}
