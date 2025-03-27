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
    public class OwnerResponseFeedbackRepository : GenericRepository<OwnerResponseFeedback>, IOwnerResponseFeedbackRepository
    {
        public OwnerResponseFeedbackRepository() { }
        public OwnerResponseFeedbackRepository(WorkHiveContext context) => _context = context;

        public async Task<OwnerResponseFeedback?> GetResponseFeedbackById(int id)
        {
            return await _context.OwnerResponseFeedbacks
                .Include(r => r.Feedback)
                .ThenInclude(f => f.Booking)
                .ThenInclude(b => b.User)
                .Include(r => r.ImageResponseFeedbacks)
                .ThenInclude(img => img.Image)
                .FirstOrDefaultAsync(fb => fb.Id == id);
        }

        public async Task<List<OwnerResponseFeedback>> GetAllResponseFeedbacks()
        {
            return await _context.OwnerResponseFeedbacks
                .Include(r => r.Feedback)
                .ThenInclude(f => f.Booking)
                .ThenInclude(b => b.User)
                .Include(r => r.ImageResponseFeedbacks)
                .ThenInclude(img => img.Image)
                .ToListAsync();
        }

    }
}
