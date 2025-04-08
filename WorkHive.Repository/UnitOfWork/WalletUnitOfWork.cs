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
    public class WalletUnitOfWork : IWalletUnitOfWork
    {
        protected WorkHiveContext _context;

        public IUserRepository User { get; private set; }
       
        public IWorkspaceRepository Workspace { get; private set; }

        public IWalletRepository Wallet { get; private set; }

        public IOwnerTransactionHistoryRepository OwnerTransactionHistory { get; private set; }

        public ITransactionHistoryRepository TransactionHistory { get; private set; }

        public IOwnerWalletRepository OwnerWallet { get; private set; }

        public IBookingRepository Booking { get; private set; }

        public IWorkspaceOwnerRepository WorkspaceOwner { get; private set; }
        
        public IOwnerWithdrawalRequestRepository OwnerWithdrawalRequest { get; private set; }

        public IOwnerNotificationRepository OwnerNotification { get; private set; }


        public WalletUnitOfWork(WorkHiveContext context)
        {
            _context = context;
            User = new UserRepository(_context);
            WorkspaceOwner = new WorkspaceOwnerRepository(_context);
            Workspace = new WorkspaceRepository(_context);
            Wallet = new WalletRepository(_context);
            OwnerTransactionHistory = new OwnerTransactionHistoryRepository(_context);
            TransactionHistory = new TransactionHistoryRepository(_context);
            OwnerWallet = new OwnerWalletRepository(_context);
            Booking = new BookingRepository(_context);
            OwnerWithdrawalRequest = new OwnerWithdrawalRequestRepository(_context);
            OwnerNotification = new OwnerNotificationRepository(_context);

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
