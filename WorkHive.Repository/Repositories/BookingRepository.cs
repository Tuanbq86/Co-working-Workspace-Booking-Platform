using Microsoft.EntityFrameworkCore;
using WorkHive.Data.Base;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;

namespace WorkHive.Repositories.Repositories;

public class BookingRepository : GenericRepository<Booking>, IBookingRepository
{
    public BookingRepository() { }
    public BookingRepository(WorkHiveContext context) => _context = context;


    public async Task<List<Booking>> GetAllBookingByUserId(int userId)
    {
        return await _context.Bookings
                                .Where(b => b.UserId.Equals(userId))
                                .Include(b => b.Payment)
                                .Include(b => b.WorkspaceTimes)
                                .Include(b => b.Workspace)
                                .ThenInclude(ws => ws.WorkspaceImages)
                                .ThenInclude(wi => wi.Image)
                                .Include(b => b.Workspace)
                                .ThenInclude(ws => ws.Owner)
                                //.Include(b => b.Promotion)
                                .Include(b => b.BookingAmenities)
                                .ThenInclude(ba => ba.Amenity)
                                .Include(b => b.BookingBeverages)
                                .ThenInclude(bb => bb.Beverage)
                                .AsNoTracking() // optimize efficiency
                                .ToListAsync();
    }

    public async Task<decimal?> GetTotalRevenueByWorkspaceIdAsync(int workspaceId, string status)
    {
        return await _context.Bookings
            .Where(b => b.WorkspaceId == workspaceId && b.Status == status)
            .SumAsync(b => b.Price);
    }

    public async Task<int> CountByWorkspaceIdAsync(int workspaceId, string status)
    {
        return await _context.Bookings
            .Where(b => b.WorkspaceId == workspaceId && b.Status == status)
            .CountAsync();
    }

    public async Task<List<Booking>> GetBookingsWithFeedbackByOwnerId(int ownerId)
    {
        return await _context.Bookings
            .Where(b => b.IsFeedback == 1 && b.Workspace.OwnerId == ownerId)
            .Include(b => b.User)
            .Include(b => b.Workspace)
            .Include(b => b.Feedbacks)
            .ToListAsync();
    }

}
