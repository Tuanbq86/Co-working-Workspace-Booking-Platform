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
    class OwnerVerifyRequestRepository : GenericRepository<OwnerVerifyRequest>, IOwnerVerifyRequestRepository
    {
        public OwnerVerifyRequestRepository() { }
        public OwnerVerifyRequestRepository(WorkHiveContext context) => _context = context;

        public async Task<List<OwnerVerifyRequest>> GetAllOwnerVerifyRequests()
        {
            return await _context.OwnerVerifyRequests
                .Include(ovr => ovr.Owner)
                .Include(ovr => ovr.User)
                .ToListAsync();
        }

        public async Task<OwnerVerifyRequest?> GetByOwnerIdAsync(int ownerId, string status)
        {
            return await _context.OwnerVerifyRequests
                .Where(x => x.OwnerId == ownerId && x.Status == status)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();
        }
    }
}
