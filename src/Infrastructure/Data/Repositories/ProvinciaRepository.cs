using Microsoft.EntityFrameworkCore;
using src.Models.CodeFirst;
using src.Repositories.Interfaces;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;
using src.Models.Mappers;

namespace src.Repositories.Implementations;

public class ProvinciaRepository : IProvinciaRepository
{
    private readonly AppDbContext _context;

    public ProvinciaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Dom.Provincia>> GetAllAsync()
    {
        IEnumerable<EF.Provincia> provinciasEF = await _context.Set<EF.Provincia>().AsNoTracking().ToListAsync();
        IEnumerable<Dom.Provincia> provinciasDom = DominioMapper.Map(provinciasEF);

        return provinciasDom;
    }

    public async Task<Dom.Provincia?> GetByIdAsync(int idProvincia)
    {
        EF.Provincia? provinciaEF = await _context.Set<EF.Provincia>()
            .AsNoTracking().FirstOrDefaultAsync(p => p.IdProvincia == idProvincia);
        if (provinciaEF == null)
        {
            return null;
        }
        return DominioMapper.Map(provinciaEF);
    }
}
