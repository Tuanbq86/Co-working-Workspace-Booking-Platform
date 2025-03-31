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

        public async Task<List<OwnerResponseFeedback>> GetResponseFeedbacksByOwnerId(int ownerId)
        {
            return await _context.OwnerResponseFeedbacks
                .Include(rf => rf.Feedback)
                    .ThenInclude(fb => fb.Booking)
                    .ThenInclude(b => b.User)
                .Include(rf => rf.ImageResponseFeedbacks)
                    .ThenInclude(imgFeedback => imgFeedback.Image)
                .Where(rf => rf.OwnerId == ownerId)
                .ToListAsync();
        }

        public async Task<List<OwnerResponseFeedback>> GetResponseFeedbacksByUserId(int userId) 
        {
            return await _context.OwnerResponseFeedbacks
                .Include(rf => rf.Feedback)
                    .ThenInclude(fb => fb.Booking)
                    .ThenInclude(b => b.User)
                .Include(rf => rf.ImageResponseFeedbacks)
                    .ThenInclude(imgFeedback => imgFeedback.Image)
                .Where(rf => rf.Feedback.Booking.User.Id == userId) 
                .ToListAsync();
        }
    }
}
