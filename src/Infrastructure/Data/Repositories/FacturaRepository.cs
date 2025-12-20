using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using src.Repositories.Interfaces;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;
using src.Models.Mappers;
using src.Interfaces;
using src.Models.Domain;
using src.Models.Common;

namespace src.Repositories.Implementations
{
    public class FacturaRepository : IFacturaRepository
    {
        private readonly EF.AppDbContext _context;

        public FacturaRepository(EF.AppDbContext context)
        {
            _context = context;
        }

        private IQueryable<EF.Factura> GetQueryFactura()
        {
            return _context.Facturas
                .Include(f => f.Compra)
                .Include(f => f.CondicionPago)
                .Include(f => f.Proveedor)
                .ThenInclude(p => p.IdDomicilioNavigation)
                .ThenInclude(d => d.IdProvinciaNavigation)
                .Include(f => f.Proveedor)
                .ThenInclude(p => p.IdCondicionPagoHabitualNavigation)
                .Include(f => f.Detalles)
                .ThenInclude(d => d.Producto);
        }

        public async Task<Dom.Factura> AddAsync(Dom.Factura factura)
        {
            var entity = DominioMapper.Map(factura);
            await _context.Facturas.AddAsync(entity);

            await _context.SaveChangesAsync();
            var facturaCreada = await GetByIdAsync(entity.IdFactura);

            if (facturaCreada == null)
            {
                factura.IdFactura = entity.IdFactura;
                return factura;
            }

            return facturaCreada;
        }

        public async Task<bool> ExisteNumeroFacturaAsync(short idProveedor, string numeroFactura)
        {
            return await _context.Facturas
                .AnyAsync(f => f.IdProveedor == idProveedor && f.NumeroFactura == numeroFactura);
        }

        public async Task<IEnumerable<Factura>> GetAllAsync()
        {
            var facturasEF = await GetQueryFactura().AsNoTracking().OrderByDescending(c => c.FechaEmision).ToListAsync();
            return DominioMapper.Map(facturasEF);
        }

        public async Task<Factura?> GetByIdAsync(int id)
        {
            var entity = await GetQueryFactura()
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.IdFactura == id);

            return entity is null ? null : DominioMapper.Map(entity);
        }

        public async Task<IEnumerable<Factura>> GetByProveedorAsync(short idProveedor)
        {
            var facturasEF = await GetQueryFactura()
                .AsNoTracking()
                .Where(f => f.IdProveedor == idProveedor)
                .OrderByDescending(f => f.FechaEmision)
                .ToListAsync();

            return DominioMapper.Map(facturasEF);
        }

        public async Task<IEnumerable<Factura>> GetPendientesPagoAsync()
        {
            var facturasEF = await GetQueryFactura()
                .AsNoTracking()
                .Where(f => !f.Pagada)
                .OrderBy(f => f.FechaEmision)
                .ToListAsync();

            return DominioMapper.Map(facturasEF);
        }

        public async Task UpdateAsync(Factura factura)
        {
            var entity = DominioMapper.Map(factura);
            _context.Facturas.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}