using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Base;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;

namespace WorkHive.Repositories.Repositories;

public class ImageRepository : GenericRepository<Image>, IImageRepository
{
    public ImageRepository() { }
    public ImageRepository(WorkHiveContext context) => _context = context;

    public async Task CreateImagesAsync(List<Image> images)
    {
        if (images == null || !images.Any()) return;

        await _context.Images.AddRangeAsync(images);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Image>> GetImagesByWorkspaceIdAsync(int workspaceId)
    {
        return await _context.Images
            .Where(i => i.WorkspaceImages.Any(wi => wi.WorkspaceId == workspaceId))
            .ToListAsync();
    }


}
