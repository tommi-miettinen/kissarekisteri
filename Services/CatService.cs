using Kissarekisteribackend.Database;
using Kissarekisteribackend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kissarekisteribackend.Services;

public class CatService
{

    private readonly KissarekisteriDbContext _dbContext;

    public CatService(KissarekisteriDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Cat> GetCatByIdAsync(int catId)
    {
        return await _dbContext.Cats.FirstOrDefaultAsync(cat => cat.Id == catId);
    }

    public async Task<List<Cat>> GetCatsAsync()
    {
        var cats = await _dbContext.Cats.ToListAsync();
        return cats;
    }
}


