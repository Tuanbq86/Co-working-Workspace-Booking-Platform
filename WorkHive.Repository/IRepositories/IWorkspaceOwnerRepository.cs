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
    public WorkspaceOwner RegisterWorkspaceOwner(string email, string phone, string password);
    public WorkspaceOwner FindWorkspaceOwnerByEmail(string email);
    public WorkspaceOwner FindWorkspaceOwnerByPhone(string phone);
    public bool FindWorkspaceOwnerByEmailOrPhone(string auth, string password);
    public bool CheckNewAndConfrimPassword(string newPassword, string confirmPassword);
    public Task<List<WorkspaceOwner>> GetOwnersByIdsAsync(List<int> ownerIds);
    public Task<List<WorkspaceOwner>> GetOwnersByUserId(int userId);
    public Task<List<WorkspaceOwner>> GetAllOwnersAsync();
    public Task<WorkspaceOwner?> GetOwnerByIdAsync(int id);
    public IQueryable<WorkspaceOwner> GetOwnerForSearch();

}
