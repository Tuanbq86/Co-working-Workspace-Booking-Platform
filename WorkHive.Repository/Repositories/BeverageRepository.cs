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

public class BeverageRepository : GenericRepository<Beverage>, IBeverageRepository
{
    public BeverageRepository() { }

    public BeverageRepository(WorkHiveContext context) => _context = context;



    public async Task<List<Beverage>> GetBeveragesByOwnerIdAsync(int OwnerId)
    {
        return await _context.Beverages
            .Where(ws => ws.OwnerId == OwnerId)
            .ToListAsync();
    }

    public async Task<List<NumberOfBookingBeveragesDTO>> GetNumberOfBookingBeverage(int OwnerId)
    {
        var result = await _context.Beverages
            .Where(b => b.OwnerId.Equals(OwnerId))
            .Select(b => new NumberOfBookingBeveragesDTO
            {
                BeverageId = b.Id,
                BeverageName = b.Name,
                Category = b.Category,
                Description = b.Description,
                Img_Url = b.ImgUrl,
                UnitPrice = b.Price,
                NumberOfBooking = _context.BookingBeverages.Count(bb => bb.BeverageId == b.Id)
            }).OrderByDescending(b => b.NumberOfBooking)
            .ToListAsync();
        return result;
    }
}
