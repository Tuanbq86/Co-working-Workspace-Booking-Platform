using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHive.Data.Base;
using WorkHive.Data.Models;
using WorkHive.Repositories.IRepositories;
using static System.Net.Mime.MediaTypeNames;

namespace WorkHive.Repositories.Repositories;

public class PriceRepository : GenericRepository<Price>, IPriceRepository
{
    public PriceRepository() { }
    public PriceRepository(WorkHiveContext context) => _context = context;

    public async Task CreatePricesAsync(List<Price> prices)
    {
        if (prices == null || !prices.Any()) return;

        await _context.Prices.AddRangeAsync(prices);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Price>> GetPricesByWorkspaceIdAsync(int workspaceId)
    {
        return await _context.Prices
                    .Where(i => i.WorkspacePrices.Any(wi => wi.WorkspaceId == workspaceId))
                    .ToListAsync();
    }
}
