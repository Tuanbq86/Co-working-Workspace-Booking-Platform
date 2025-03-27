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


        public async Task<OwnerResponseFeedback> GetResponseFeedbackById(int id)
        {
            return await _context.OwnerResponseFeedbacks
                .Include(fb => fb.ImageResponseFeedbacks)
                .ThenInclude(f => f.Image)
                .Include(fb => fb.Feedback)
                .ThenInclude(fb => fb.User).ThenInclude(fb => fb.Bookings).ThenInclude(fb => fb.Workspace)
                .FirstOrDefaultAsync(fb => fb.Id == id);
        }

    }
}
