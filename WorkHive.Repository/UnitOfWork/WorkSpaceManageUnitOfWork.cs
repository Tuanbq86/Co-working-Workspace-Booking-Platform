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

        public IUserRepository User { get; private set; }
        public IFacilityRepository Facility { get; private set; }

        public IPolicyRepository Policy { get; private set; }

        public IAmenityRepository Amenity { get; private set; }

        public IBeverageRepository Beverage { get; private set; }

        public IImageRepository Image { get; private set; }
        public IPriceRepository Price { get; private set; } 

        public IWorkspacePriceRepository WorkspacePrice { get; private set; }

        public IWorkspaceImageRepository WorkspaceImage { get; private set; }   

        public IWorkspaceRepository Workspace { get; private set; }

        public IWorkspaceFacilityRepository WorkspaceFacility { get; private set; }

        public IWorkspacePolicyRepository WorkspacePolicy { get; private set; }

        public IPromotionRepository Promotion { get; private set; }
        
        public IBookingRepository Booking { get; private set; }

        public IWorkspaceOwnerRepository WorkspaceOwner { get; private set; }

        public IOwnerNotificationRepository OwnerNotification { get; private set; }

        public IOwnerVerifyRequestRepository OwnerVerifyRequest { get; private set; }


        public WorkSpaceManageUnitOfWork(WorkHiveContext context)
        {
            _context = context;
            User = new UserRepository(_context);
            Facility = new FacilityRepository(_context);
            Policy = new PolicyRepository(_context);
            Amenity = new AmenityRepository(_context);
            Beverage = new BeverageRepository(_context);
            Workspace = new WorkspaceRepository(_context);
            WorkspaceOwner = new WorkspaceOwnerRepository(_context);
            WorkspaceImage = new WorkspaceImageRepository(_context);
            WorkspacePrice = new WorkspacePriceRepository(_context);
            WorkspaceFacility = new WorkspaceFacilityRepository(_context);
            WorkspacePolicy = new WorkspacePolicyRepository(_context);
            Price = new PriceRepository(_context);
            Image = new ImageRepository(_context);
            Booking = new BookingRepository(_context);
            Promotion = new PromotionRepository(_context);
            OwnerNotification = new OwnerNotificationRepository(_context);
            OwnerVerifyRequest = new OwnerVerifyRequestRepository(_context);

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
