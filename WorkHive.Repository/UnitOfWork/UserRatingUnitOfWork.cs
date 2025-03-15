using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;
using WorkHive.Repositories.IUnitOfWork;
using WorkHive.Repositories.Repositories;

namespace WorkHive.Repositories.UnitOfWork;
public class UserRatingUnitOfWork : IUserRatingUnitOfWork
{
    private readonly WorkHiveContext _context;
    public IBookingRepository booking { get; private set; }
    public IUserRepository user { get; private set; }
    public IRatingRepository rating { get; private set; }
    public IWorkspaceRatingRepository workspaceRating { get; private set; }
    public IWorkspaceRatingImageRepository workspaceRatingImage { get; private set; }
    public IImageRepository image { get; private set; }
    public IWorkspaceRepository workspace { get; private set; }
    public IWorkspaceOwnerRepository owner { get; private set; }

    public UserRatingUnitOfWork(WorkHiveContext context)
    {
        _context = context;
        booking = new BookingRepository(_context);
        user = new UserRepository(_context);
        rating = new RatingRepository(_context);
        workspaceRating = new WorkspaceRatingRepository(_context);
        workspaceRatingImage = new WorkspaceRatingImageRepository(_context);
        workspace = new WorkspaceRepository(_context);
        owner = new WorkspaceOwnerRepository(_context);
        image = new ImageRepository(_context);
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
