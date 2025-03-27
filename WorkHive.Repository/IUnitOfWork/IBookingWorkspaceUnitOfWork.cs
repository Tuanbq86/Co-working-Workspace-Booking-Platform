using WorkHive.Repositories.IRepositories;

namespace WorkHive.Repositories.IUnitOfWork;

public interface IBookingWorkspaceUnitOfWork
{
    IBookingRepository booking { get; }
    IPaymentMethodRepository payment { get; }
    IPromotionRepository promotion { get; }
    IWorkspaceRepository workspace { get; }
    IBookingAmenityRepository bookAmenity { get; }
    IBookingBeverageRepository bookBeverage { get; }
    IAmenityRepository amenity { get; }
    IBeverageRepository beverage { get; }
    IWorkspaceTimeRepository workspaceTime { get; }
    IOwnerWalletRepository ownerWallet { get; }
    IWalletRepository wallet { get; }
    IOwnerNotificationRepository ownerNotification { get; }
    IWorkspaceOwnerRepository Owner { get; }
    public int Save();
    public Task<int> SaveAsync();
}
