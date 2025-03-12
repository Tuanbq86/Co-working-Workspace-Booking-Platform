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

public class BookingAmenityRepository : GenericRepository<BookingAmenity>, IBookingAmenityRepository
{
    public BookingAmenityRepository() { }
    public BookingAmenityRepository(WorkHiveContext context) => _context = context;

    public async Task<List<BookingAmenity>> GetAllBookingAmenityByBookingId(int bookingId)
    {
        return await _context.BookingAmenities
            .Where(ba => ba.BookingId.Equals(bookingId))
            .Include(ba => ba.Amenity).ToListAsync();
    }

}
