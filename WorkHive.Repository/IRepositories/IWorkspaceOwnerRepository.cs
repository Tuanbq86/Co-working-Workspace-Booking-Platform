using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Base;
using WorkHive.Data.Models;

namespace WorkHive.Repositories.IRepositories;

public interface IWorkspaceOwnerRepository : IGenericRepository<WorkspaceOwner>
{
    public WorkspaceOwner RegisterOwnerByPhoneAndEmail(string email,
        string phone, string password);
    public WorkspaceOwner FindOwnerByEmail(string email);
    public WorkspaceOwner FindOwnerByPhone(string phone);
    public bool FindOwnerByEmailOrPhone(string auth, string password);
    public Task<List<WorkspaceOwner>> GetOwnersByIdsAsync(List<int> ownerIds);
}
