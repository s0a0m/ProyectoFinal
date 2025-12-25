using Microsoft.EntityFrameworkCore;
using src.Interfaces;
using src.Models.CodeFirst;
using src.Models.Mappers;
using src.Repositories.Interfaces;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;

namespace src.Infrastructure.Repositories;

public class OrdenPagoRepository : IOrdenPagoRepository
{
    private readonly AppDbContext _context;

    public OrdenPagoRepository(AppDbContext context)
    {
        _context = context;
    }
    private IQueryable<EF.OrdenPago> GetQueryOrdenPago()
    {
        return _context.OrdenesPago
                .AsNoTracking()
                .Include(o => o.Proveedor)
                    .ThenInclude(p => p.IdCondicionPagoHabitualNavigation)
                .Include(o => o.Proveedor)
                    .ThenInclude(p => p.IdDomicilioNavigation)
                        .ThenInclude(d => d.IdProvinciaNavigation)
                .Include(o => o.Detalles)
                    .ThenInclude(d => d.Factura)
                        .ThenInclude(f => f.CondicionPago)
                .Include(o => o.Detalles)
                    .ThenInclude(d => d.Factura)
                        .ThenInclude(f => f.Proveedor)
                            .ThenInclude(p => p.IdCondicionPagoHabitualNavigation)
                .Include(o => o.Detalles)
                    .ThenInclude(d => d.Factura)
                        .ThenInclude(f => f.Proveedor)
                            .ThenInclude(p => p.IdDomicilioNavigation)
                                .ThenInclude(d => d.IdProvinciaNavigation);
    }

    public async Task<IEnumerable<Dom.OrdenPago>> GetAllAsync()
    {
        var listEf = await GetQueryOrdenPago()
            .OrderByDescending(o => o.FechaPago)
            .ThenByDescending(o => o.IdOrdenPago)
            .ToListAsync();

        return DominioMapper.Map(listEf);
    }

    public async Task<Dom.OrdenPago?> GetByIdWithDetallesAsync(int id)
    {
        var entityEf = await GetQueryOrdenPago()
            .FirstOrDefaultAsync(o => o.IdOrdenPago == id);

        if (entityEf == null) return null;

        return DominioMapper.Map(entityEf);
    }

    public async Task CreateAsync(Dom.OrdenPago ordenDom)
    {
        var entityEf = DominioMapper.Map(ordenDom);
        _context.OrdenesPago.Add(entityEf);
        await _context.SaveChangesAsync();
        ordenDom.IdOrdenPago = entityEf.IdOrdenPago;
    }

    public async Task UpdateAsync(Dom.OrdenPago ordenDom)
    {
        var entityEf = await _context.OrdenesPago
            .FirstOrDefaultAsync(o => o.IdOrdenPago == ordenDom.IdOrdenPago);

        if (entityEf == null) return;
        entityEf.Enviada = ordenDom.Enviada;
        entityEf.FechaPago = ordenDom.FechaPago;
        entityEf.MontoTotal = ordenDom.MontoTotal;

        await _context.SaveChangesAsync();
    }
}