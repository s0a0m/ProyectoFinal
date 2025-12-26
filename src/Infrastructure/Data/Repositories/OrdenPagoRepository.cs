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
        var ordenEf = await _context.OrdenesPago
            .Include(o => o.Detalles)
            .FirstOrDefaultAsync(o => o.IdOrdenPago == ordenDom.IdOrdenPago);

        if (ordenEf == null) throw new KeyNotFoundException($"Orden {ordenDom.IdOrdenPago} no encontrada");

        ordenEf.IdProveedor = ordenDom.IdProveedor;
        ordenEf.FechaPago = ordenDom.FechaPago;
        ordenEf.Enviada = ordenDom.Enviada;
        ordenEf.MontoTotal = ordenDom.MontoTotal;

        var idsNuevos = ordenDom.Detalles.Select(d => d.IdFactura).ToList();
        var borrar = ordenEf.Detalles.Where(d => !idsNuevos.Contains(d.IdFactura)).ToList();
        foreach (var item in borrar) _context.Remove(item);

        foreach (var detalleDom in ordenDom.Detalles)
        {
            var existente = ordenEf.Detalles.FirstOrDefault(d => d.IdFactura == detalleDom.IdFactura);

            if (existente != null)
            {
                existente.MontoAplicado = detalleDom.MontoAplicado;
            }
            else
            {
                ordenEf.Detalles.Add(new EF.PagoDetalle
                {
                    IdFactura = detalleDom.IdFactura,
                    MontoAplicado = detalleDom.MontoAplicado
                });
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Dom.OrdenPago>> GetPagosPorFacturaIdAsync(int idFactura)
    {
        var pagosEf = await GetQueryOrdenPago()
            .Where(op => op.Detalles.Any(d => d.IdFactura == idFactura))
            .ToListAsync();

        return DominioMapper.Map(pagosEf);
    }

    public async Task<IEnumerable<Dom.OrdenPago>> GetByProveedorAsync(short idProveedor)
        {
            var OrdenEF = await GetQueryOrdenPago()
                .AsNoTracking()
                .Where(f => f.IdProveedor == idProveedor && f.Enviada == true)
                .ToListAsync();
            return DominioMapper.Map(OrdenEF);
        }

}