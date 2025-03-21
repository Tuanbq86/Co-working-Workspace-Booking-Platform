using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Repositories.IRepositories;
using WorkHive.Repositories.Repositories;

namespace WorkHive.Repositories.IUnitOfWork
{
    public interface IWalletUnitOfWork
    {
        IUserRepository User { get; }
        IBookingRepository Booking { get; }
        IWorkspaceOwnerRepository WorkspaceOwner { get; }
        IWorkspaceRepository Workspace { get; }
        IWalletRepository Wallet { get; }
        IOwnerTransactionHistoryRepository OwnerTransactionHistory { get; }
        ITransactionHistoryRepository TransactionHistory { get; }
        IOwnerWalletRepository OwnerWallet { get; }



        int Save();
        Task<int> SaveAsync();


    }
}
