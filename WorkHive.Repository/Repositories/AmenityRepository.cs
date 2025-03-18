using Microsoft.EntityFrameworkCore;
using WorkHive.Data.Base;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;

namespace WorkHive.Repositories.Repositories;

class AmenityRepository : GenericRepository<Amenity>, IAmenityRepository
{
    public AmenityRepository() { }
    public AmenityRepository(WorkHiveContext context) => _context =  context;

    public async Task<List<Amenity>> GetAmenitiesByOwnerIdAsync(int ownerId)
    {
        return await _context.Amenities
            .Where(ws => ws.OwnerId == ownerId)
            .ToListAsync();
    }

    public async Task<List<NumberOfBookingAmenitiesDTO>> GetNumberOfBookingAmenity(int OwnerId)
    {
        var result = await _context.Amenities
            .Where(a => a.OwnerId.Equals(OwnerId))
            .Select(a => new NumberOfBookingAmenitiesDTO
            {
                AmenityId = a.Id,
                AmenityName = a.Name,
                Category = a.Category,
                Description = a.Description,
                Img_Url = a.ImgUrl,
                UnitPrice = a.Price,
                NumberOfBooking = _context.BookingAmenities.Count(ba => ba.AmenityId == a.Id)
            }).OrderByDescending(a => a.NumberOfBooking)
            .ToListAsync();
        return result;

    }
}
