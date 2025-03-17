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
    public class OwnerResponseFeedbackRepository : GenericRepository<OwnerResponeFeedback>, IOwnerResponseFeedbackRepository
    {
        public OwnerResponseFeedbackRepository() { }
        public OwnerResponseFeedbackRepository(WorkHiveContext context) => _context = context;


        public async Task<OwnerResponeFeedback?> GetResponseFeedbackById(int id)
        {
            return await _context.OwnerResponeFeedbacks.Include(fb => fb.ImageResponseFeedbacks).ThenInclude(f => f.Image).FirstOrDefaultAsync(fb => fb.Id == id);
        }

        //To do object method
    }
}
