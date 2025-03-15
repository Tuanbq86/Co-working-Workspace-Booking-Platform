using WorkHive.Repositories.IRepositories;

namespace WorkHive.Repositories.IUnitOfWork;

public interface IUserRatingUnitOfWork
{
    IBookingRepository booking { get; }
    IUserRepository user { get; }
    IRatingRepository rating { get; }
    IWorkspaceRatingRepository workspaceRating { get; }
    IWorkspaceRatingImageRepository workspaceRatingImage { get; }
    IImageRepository image { get; }
    IWorkspaceRepository workspace { get; }
    IWorkspaceOwnerRepository owner { get; }
    public int Save();
    public Task<int> SaveAsync();
}
