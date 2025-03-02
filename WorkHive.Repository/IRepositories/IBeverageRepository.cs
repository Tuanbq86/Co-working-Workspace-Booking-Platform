using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Base;
using WorkHive.Data.Models;

namespace WorkHive.Repositories.IRepositories;

public interface IBeverageRepository : IGenericRepository<Beverage>
{
    public Task<List<Beverage>> GetBeveragesByWorkSpaceIdAsync(int workspaceId);
}
