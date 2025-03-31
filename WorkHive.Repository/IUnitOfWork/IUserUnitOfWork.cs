using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Repositories.IRepositories;
namespace WorkHive.Repositories.IUnitOfWork;

//Unit of work is used for one transaction for database
public interface IUserUnitOfWork
{
    IUserRepository User { get; }
    IWorkspaceOwnerRepository Owner { get; }
    IRoleRepository Role { get; }
    IWalletRepository Wallet { get; }
    ICustomerWalletRepository CustomerWallet { get; }
    IUserTransactionHistoryRepository UserTransactionHistory { get; }
    ITransactionHistoryRepository TransactionHistory { get; }
    IOwnerTransactionHistoryRepository OwnerTransactionHistory { get; }
    IUserNotificationRepository UserNotification { get; }
    IUserPasswordResetTokenRepository PasswordResetToken { get; }
    public int Save();
    public Task<int> SaveAsync();
}
