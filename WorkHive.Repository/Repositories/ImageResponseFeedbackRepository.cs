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
    public class ImageResponseFeedbackRepository : GenericRepository<ImageResponseFeedback>, IImageResponseFeedbackRepository
    {
        public ImageResponseFeedbackRepository(){ }
        public ImageResponseFeedbackRepository(WorkHiveContext context) => _context = context;

        public async Task CreateImageResponseFeedbackAsync(List<ImageResponseFeedback> imageResponseFeedbacks)
        {
            if (imageResponseFeedbacks == null || !imageResponseFeedbacks.Any()) return;

            await _context.ImageResponseFeedbacks.AddRangeAsync(imageResponseFeedbacks);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ImageResponseFeedback>> GetImageResponseFeedbacksByFeedbackIdAsync(int responseId)
        {
            return await _context.ImageResponseFeedbacks
                   .Where(i => i.ResponseFeedbackId == responseId)
                   .ToListAsync();
        }
    }
}
