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


    public async Task DeleteImagesByIdsAsync(List<int> imageIds)
    {
        var imagesToDelete = await _context.Images
            .Where(img => imageIds.Contains(img.Id))
            .ToListAsync();

        if (imagesToDelete.Any())
        {
            _context.Images.RemoveRange(imagesToDelete);
            await _context.SaveChangesAsync();
        }
    }
}
