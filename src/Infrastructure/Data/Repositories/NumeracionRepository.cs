using Microsoft.EntityFrameworkCore;
using src.Models.CodeFirst;
using src.Repositories.Interfaces;

namespace src.Repositories.Implementations;

public class NumeracionRepository : INumeracionRepository
{
    private readonly AppDbContext _context;

    public NumeracionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string?> ObtenerUltimoNumeroAsync(string prefijo, string tipo)
    {
        return tipo switch
        {
            "NC" or "ND" => await _context
                .Comprobantes.Where(c => c.Numero.StartsWith(prefijo))
                .OrderByDescending(c => c.Numero)
                .Select(c => c.Numero)
                .FirstOrDefaultAsync(),

            "OP" => await _context
                .OrdenesPago.Where(o => o.Numero.StartsWith(prefijo))
                .OrderByDescending(o => o.Numero)
                .Select(o => o.Numero)
                .FirstOrDefaultAsync(),

            _ => null,
        };
    }
}
