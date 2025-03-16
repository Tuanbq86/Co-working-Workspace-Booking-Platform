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
    public class ImageFeedbackRepository : GenericRepository<ImageFeedback>, IImageFeedbackRepository
    {
        public ImageFeedbackRepository() { }
        public ImageFeedbackRepository(WorkHiveContext context) => _context = context;

        public async Task CreateImageFeedbackAsync(List<ImageFeedback> imageFeedbacks)
        {
            if (imageFeedbacks == null || !imageFeedbacks.Any()) return;

            await _context.ImageFeedbacks.AddRangeAsync(imageFeedbacks);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ImageFeedback>> GetImageFeedbacksByFeedbackIdAsync(int feedbackId)
        {
            return await _context.ImageFeedbacks
                   .Where(i => i.FeedbackId == feedbackId)
                   .ToListAsync();
        }

        //To do object method


    }
}
