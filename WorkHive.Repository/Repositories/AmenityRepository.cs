using WorkHive.Data.Base;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;

namespace WorkHive.Repositories.Repositories;

class AmenityRepository : GenericRepository<Amenity>, IAmenityRepository
{
    public AmenityRepository() { }
    public AmenityRepository(WorkHiveContext context) => _context =  context;

    //To do object method
}
