using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Base;
using WorkHive.Data.Models;

namespace WorkHive.Repositories.IRepositories;

public interface IFacilityRepository : IGenericRepository<Facility>
{
    public Task CreateFacilitiesAsync(List<Facility> facilities);
    public Task<List<Facility>> GetFacilitiesByWorkspaceIdAsync(int workspaceId);
    public Task DeleteFacilitiesByIdsAsync(List<int> facilityIds);
}
