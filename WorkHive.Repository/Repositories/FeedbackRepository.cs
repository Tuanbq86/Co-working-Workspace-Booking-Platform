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
            .Include(f => f.Booking).ThenInclude(b => b.Workspace).ThenInclude(w => w.Owner)
            .FirstOrDefaultAsync(fb => fb.Id == id);
    }

    public async Task<List<Feedback>> GetAllFeedbacks()
    {
        return await _context.Feedbacks
            .Include(f => f.Booking)
                .ThenInclude(b => b.Workspace)
                    .ThenInclude(w => w.Owner)
            .Include(f => f.ImageFeedbacks)
                .ThenInclude(ifb => ifb.Image)
            .ToListAsync();
    }

    public async Task<List<Feedback>> GetFeedbacksByUserId(int userId)
    {
        return await _context.Feedbacks
            .Include(f => f.Booking)
                .ThenInclude(b => b.Workspace)
                .ThenInclude(w => w.Owner)
            .Include(f => f.ImageFeedbacks)
                .ThenInclude(imgFeedback => imgFeedback.Image)
            .Where(f => f.UserId == userId)
            .ToListAsync();
    }

    public async Task<List<Feedback>> GetFeedbacksByOwnerId(int ownerId)
    {
        return await _context.Feedbacks
            .Include(fb => fb.Booking)
                .ThenInclude(b => b.Workspace)
            .Include(fb => fb.ImageFeedbacks)
                .ThenInclude(imgFeedback => imgFeedback.Image)
            .Where(fb => fb.Booking.Workspace.Owner.Id == ownerId) 
            .ToListAsync();


    }

    public async Task<Feedback?> GetFirstFeedbackByBookingId(int bookingId)
    {
        return await _context.Feedbacks.Include(f => f.ImageFeedbacks)
            .ThenInclude(f => f.Image)
            .Include(f => f.Booking).ThenInclude(b => b.Workspace).ThenInclude(w => w.Owner)
            .Where(f => f.BookingId == bookingId)
            .FirstOrDefaultAsync();
    }

}