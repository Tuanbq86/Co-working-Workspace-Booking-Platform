using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Base;
using WorkHive.Data.Models;

namespace WorkHive.Repositories.IRepositories;

public interface IWorkspaceRepository : IGenericRepository<Workspace>
{
    public Task<List<Workspace>> GetAllWorkSpaceByOwnerIdAsync(int ownerId);
    public Task<List<Workspace>> GetAllWorkSpaceAsync();
    public Task<Workspace?> GetWorkSpaceById(int Id);
    public Task<Workspace> GetWorkspaceByIdForTime(int workspaceId);
    public IQueryable<Workspace> GetWorkspaceForSearch();
    public Task<Workspace?> GetByNameAsync(string name);
    public Task<List<Workspace>> GetByOwnerIdAsync(int ownerId);
    public Task<List<Workspace>> GetWorkspaceByRateSearch();
    public IQueryable<Workspace> GetWorkspaceByCategorySearch(string Category);
    public Task<List<Price>> GetPricesByWorkspaceIdAsync(int workspaceId);
}
