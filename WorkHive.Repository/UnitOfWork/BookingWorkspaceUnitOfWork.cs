using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Repositories.Repositories;

namespace WorkHive.Repositories.UnitOfWork
{
    public class BookingWorkspaceUnitOfWork : IBookingWorkspaceUnitOfWork
    {
        private readonly WorkHiveContext _context;
        public IBookingRepository booking { get; private set; }

        public IPaymentRepository payment { get; private set; }

        public IPromotionRepository promotion { get; private set; }

        public IWorkspaceRepository workspace { get; private set; }

        public IBookingAmenityRepository bookAmenity { get; private set; }

        public IBookingBeverageRepository bookBeverage { get; private set; }
        
        public IAmenityRepository amenity { get; private set; }

        public IBeverageRepository beverage { get; private set; }

        public IWorkspaceTimeRepository workspaceTime { get; private set; }

        public BookingWorkspaceUnitOfWork(WorkHiveContext context)
        {
            _context = context;
            booking = new BookingRepository(_context);
            payment = new PaymentRepository(_context);
            promotion = new PromotionRepository(_context);
            workspace = new WorkspaceRepository(_context);
            bookAmenity = new BookingAmenityRepository(_context);
            bookBeverage = new BookingBeverageRepository(_context);
            amenity = new AmenityRepository(_context);
            beverage = new BeverageRepository(_context);
            workspaceTime = new WorkspaceTimeRepository(_context);
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
