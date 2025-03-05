using Microsoft.EntityFrameworkCore;
using WorkHive.Data.Base;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;

namespace WorkHive.Repositories.Repositories;

class AmenityRepository : GenericRepository<Amenity>, IAmenityRepository
{
    public AmenityRepository() { }
    public AmenityRepository(WorkHiveContext context) => _context =  context;

    public async Task<List<Amenity>> GetAmenitiesByWorkSpaceIdAsync(int workspaceId)
    {
        return await _context.Amenities
            .Where(ws => ws.WorkspaceId == workspaceId)
            .ToListAsync();
    }

}
