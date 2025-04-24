using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Base;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;

namespace WorkHive.Repositories.Repositories
{
    public class OwnerWithdrawalRequestRepository : GenericRepository<OwnerWithdrawalRequest>, IOwnerWithdrawalRequestRepository
    {
        public OwnerWithdrawalRequestRepository() { }
        public OwnerWithdrawalRequestRepository(WorkHiveContext context) => _context = context;

        public async Task<List<OwnerWithdrawalRequest>> GetByOwnerIdAsync(int ownerId)
        {
            return await _context.OwnerWithdrawalRequests
                .Include(r => r.WorkspaceOwner)
                .ThenInclude(r => r.OwnerWallets)
                .ThenInclude(r => r.Wallet)
                .Where(r => r.WorkspaceOwnerId == ownerId)
                .ToListAsync();
        } 
        public async Task<OwnerWithdrawalRequest?> GetWithdrawalRequestByIdAsync(int id)
        {
            return await _context.OwnerWithdrawalRequests
                .Include(r => r.WorkspaceOwner)
                .ThenInclude(r => r.OwnerWallets)
                .ThenInclude(r => r.Wallet)
                .Where(r => r.Id == id)
                .FirstOrDefaultAsync();
        }

    }
}
