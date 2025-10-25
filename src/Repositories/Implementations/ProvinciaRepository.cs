using Microsoft.EntityFrameworkCore;
using src.Repositories.Interfaces;
using Dom = src.Models.Domain;

namespace src.Repositories.Implementations;

public class ProvinciaRepository : IProvinciaRepository
{
    private readonly DbContext _context;

    public ProvinciaRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Dom.Provincia>> GetAllAsync()
    {
        return await _context.Set<Dom.Provincia>().ToListAsync();
    }
}
