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

public class FeedbackRepository : GenericRepository<Feedback>, IFeedbackRepository
{
    public FeedbackRepository() { }
    public FeedbackRepository(WorkHiveContext context) => _context = context;

    public async Task<Feedback?> GetFeedbackById(int id)
    {
        return await _context.Feedbacks.Include(fb => fb.ImageFeedbacks)
            .ThenInclude(f => f.Image)
            .Include(f => f.Booking).ThenInclude(b => b.Workspace)
            .Include(f => f.Owner)
            .FirstOrDefaultAsync(fb => fb.Id == id);
    }

    //To do object method


}
