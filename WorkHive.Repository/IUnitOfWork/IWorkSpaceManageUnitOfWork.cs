using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Repositories.IRepositories;

namespace WorkHive.Repositories.IUnitOfWork
{
    public interface IWorkSpaceManageUnitOfWork
    {
        IFacilityRepository Facility { get; }
        IPolicyRepository Policy { get; }
        IAmenityRepository Amenity { get; }
        IBeverageRepository Beverage { get; }
        IWorkspaceRepository Workspace { get; }

        public int Save();
        public Task<int> SaveAsync();
    }
}
