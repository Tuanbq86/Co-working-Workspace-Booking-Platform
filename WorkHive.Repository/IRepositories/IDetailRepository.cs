using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Base;
using WorkHive.Data.Models;

namespace WorkHive.Repositories.IRepositories
{
    public interface IDetailRepository : IGenericRepository<Detail>
    {
        Task<List<Detail>> GetDetailsByWorkspaceIdAsync(int workspaceId);
        Task CreateDetailsAsync(List<Detail> Details);
    }
}
