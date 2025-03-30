using WorkHive.Data.Base;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;

namespace WorkHive.Repositories.Repositories;

public class OwnerPasswordResetTokenRepository : GenericRepository<OwnerPasswordResetToken>, IOwnerPasswordResetTokenRepository
{
    public OwnerPasswordResetTokenRepository() { }
    public OwnerPasswordResetTokenRepository(WorkHiveContext context) => _context = context;
}
