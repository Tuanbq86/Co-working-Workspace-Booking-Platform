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

public class RatingRepository : GenericRepository<Rating>, IRatingRepository
{
    public RatingRepository() { }
    public RatingRepository(WorkHiveContext context) => _context = context;

    public async Task<List<Rating>> GetAllRatingByUserId(int userId)
    {
        return await _context.Ratings
            .Where(r => r.UserId.Equals(userId))
            .Include(r => r.WorkspaceRatings)
            .ThenInclude(wr => wr.Workspace)
            .ThenInclude(w => w.Owner)
            .Include(r => r.WorkspaceRatings)
            .ThenInclude(wr => wr.WorkspaceRatingImages)
            .ThenInclude(wri => wri.Image)
            .ToListAsync();
    }
}
