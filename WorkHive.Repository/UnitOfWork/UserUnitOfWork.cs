using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Repositories.Repositories;

namespace WorkHive.Repositories.UnitOfWork;

public class UserUnitOfWork : IUserUnitOfWork
{
    protected WorkHiveContext _context;
    public IUserRepository User { get; private set; }
    public IRoleRepository Role { get; private set; }
    public IWalletRepository Wallet { get; private set; }
    public ICustomerWalletRepository CustomerWallet { get; private set; }
    public IUserTransactionHistoryRepository UserTransactionHistory { get; private set; }
    public ITransactionHistoryRepository TransactionHistory { get; private set; }
    public IWorkspaceOwnerRepository Owner { get; private set; }
    public IOwnerTransactionHistoryRepository OwnerTransactionHistory { get; private set; }
    public IUserNotificationRepository UserNotification { get; private set; }
    public IUserPasswordResetTokenRepository PasswordResetToken { get; private set; }

    public UserUnitOfWork(WorkHiveContext context)
    {
        _context = context;
        User = new UserRepository(_context);
        Role = new RoleRepository(_context);
        Wallet = new WalletRepository(_context);
        CustomerWallet = new CustomerWalletRepository(_context);
        UserTransactionHistory = new UserTransactionHistoryRepository(_context);
        TransactionHistory = new TransactionHistoryRepository(_context);
        Owner = new WorkspaceOwnerRepository(_context);
        OwnerTransactionHistory = new OwnerTransactionHistoryRepository(_context);
        UserNotification = new UserNotificationRepository(_context);
        PasswordResetToken = new UserPasswordResetTokenRepository(_context);
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
