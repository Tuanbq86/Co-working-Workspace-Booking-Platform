using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Base;
using WorkHive.Data.Models;

namespace WorkHive.Repositories.IRepositories;

public interface IBookingRepository : IGenericRepository<Booking>
{
    public Task<List<Booking>> GetAllBookingByUserId(int userId);

    public Task<decimal?> GetTotalRevenueByWorkspaceIdAsync(int workspaceId, string status);
}
