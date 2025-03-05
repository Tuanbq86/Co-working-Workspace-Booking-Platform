using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Repositories.Repositories;

namespace WorkHive.Repositories.UnitOfWork
{
    public class WorkSpaceManageUnitOfWork : IWorkSpaceManageUnitOfWork
    {
        protected WorkHiveContext _context;
        public IFacilityRepository Facility { get; private set; }

        public IPolicyRepository Policy { get; private set; }

        public IAmenityRepository Amenity { get; private set; }

        public IBeverageRepository Beverage { get; private set; }

        public IWorkspaceRepository Workspace { get; private set; }

        public IWorkspaceOwnerRepository WorkspaceOwner { get; private set; }


        public WorkSpaceManageUnitOfWork(WorkHiveContext context)
        {
            _context = context;
            Facility = new FacilityRepository(_context);
            Policy = new PolicyRepository(_context);
            Amenity = new AmenityRepository(_context);
            Beverage = new BeverageRepository(_context);
            Workspace = new WorkspaceRepository(_context);
            WorkspaceOwner = new WorkspaceOwnerRepository(_context);
        }
        public int Save()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
