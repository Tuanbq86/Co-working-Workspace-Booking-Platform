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
    public class FeedbackManageUnitOfWork : IFeedbackManageUnitOfWork
    {
        protected WorkHiveContext _context;
        public IFeedbackRepository Feedback { get; private set; }
        public IOwnerResponseFeedbackRepository OwnerResponseFeedback { get; private set; }
        public IImageFeedbackRepository ImageFeedback { get; private set; }
        public IImageRepository Image { get; private set; }
        public IImageResponseFeedbackRepository ImageResponseFeedback { get; private set; }
        public IUserRepository User { get; private set; }
        public IWorkspaceOwnerRepository WorkspaceOwner { get; private set; }
        public IBookingRepository Booking { get; private set; }
        public IOwnerNotificationRepository OwnerNotification { get; private set; }
        public IUserNotificationRepository UserNotification { get; private set; }


        public FeedbackManageUnitOfWork(WorkHiveContext context)
        {
            _context = context;
            Feedback = new FeedbackRepository(_context);
            OwnerResponseFeedback = new OwnerResponseFeedbackRepository(_context);
            ImageFeedback = new ImageFeedbackRepository(_context);
            Image = new ImageRepository(_context);
            ImageResponseFeedback = new ImageResponseFeedbackRepository(_context);
            User = new UserRepository(_context);
            WorkspaceOwner = new WorkspaceOwnerRepository(_context);
            Booking = new BookingRepository(_context);
            OwnerNotification = new OwnerNotificationRepository(_context);
            UserNotification = new UserNotificationRepository(_context);
        }
        public int Save()
        {
            return _context.SaveChanges();
        }
        public Task<int> SaveAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
