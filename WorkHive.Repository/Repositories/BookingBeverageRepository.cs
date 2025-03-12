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

public class BookingBeverageRepository : GenericRepository<BookingBeverage>, IBookingBeverageRepository
{
    public BookingBeverageRepository() { }
    public BookingBeverageRepository(WorkHiveContext context) => _context = context;

    public async Task<List<BookingBeverage>> GetAllBookingBeverageByBookingId(int bookingId)
    {
        return await _context.BookingBeverages
            .Where(bb => bb.BookingWorkspaceId.Equals(bookingId))
            .Include(bb => bb.Beverage).ToListAsync();
    }

}
