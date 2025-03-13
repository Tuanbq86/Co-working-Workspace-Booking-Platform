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
        IPriceRepository Price { get; }
        IImageRepository Image { get; }
        IWorkspaceImageRepository WorkspaceImage { get; }
        IWorkspacePriceRepository WorkspacePrice { get; }
        IWorkspaceFacilityRepository WorkspaceFacility { get; }
        IWorkspacePolicyRepository WorkspacePolicy { get; }
        IWorkspaceOwnerRepository WorkspaceOwner { get; }


        public int Save();
        public Task<int> SaveAsync();
    }
}
