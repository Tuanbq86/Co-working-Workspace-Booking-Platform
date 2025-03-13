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

public class FacilityRepository : GenericRepository<Facility>, IFacilityRepository
{
    public FacilityRepository() { }
    public FacilityRepository(WorkHiveContext context) => _context = context;

    public async Task CreateFacilitiesAsync(List<Facility> facilities)
    {
        if (facilities == null || !facilities.Any()) return;

        await _context.Facilities.AddRangeAsync(facilities);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Facility>> GetFacilitiesByWorkspaceIdAsync(int workspaceId)
    {
        return await _context.Facilities
            .Where(i => i.WorkspaceFacilities.Any(wi => wi.WorkspaceId == workspaceId))
            .ToListAsync();
    }

}
