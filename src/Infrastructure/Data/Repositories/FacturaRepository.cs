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

       // En src/Repositories/Implementations/FacturaRepository.cs

        public async Task UpdateAsync(Dom.Factura factura)
        {
            var entity = await _context.Facturas
                .Include(f => f.Detalles) // Traemos los detalles viejos
                .FirstOrDefaultAsync(f => f.IdFactura == factura.IdFactura);

            if (entity == null) throw new Exception("Factura no encontrada para actualizar");

            // 1. Actualizar Cabecera
            entity.NumeroFactura = factura.NumeroFactura;
            entity.FechaEmision = factura.FechaEmision;
            // entity.Pagada = factura.Pagada; // Generalmente no se edita el pago desde aquí, pero si quieres déjalo
            entity.TotalFacturado = factura.TotalFacturado;

            if (factura.CondicionPago is not null)
                entity.IdCondicionPagoUsada = factura.CondicionPago.IdCondicionPago;

            // 2. Actualizar Detalles (Estrategia: Borrar y Reinsertar para evitar conflictos de tracking)
            // Borramos los actuales de la BD
            _context.DetallesFactura.RemoveRange(entity.Detalles);
            
            // Mapeamos los nuevos desde el dominio a entidades EF
            var nuevosDetallesEF = factura.Detalles.Select(d => new EF.DetalleFactura
            {
                IdFactura = entity.IdFactura, // Vinculamos al padre
                IdProducto = (short)d.Producto.IdProducto,
                Cantidad = d.Cantidad,
                PrecioBruto = d.PrecioBruto,
                PorcentajeDescuento = d.PorcentajeDescuento,
                PrecioNeto = d.PrecioNeto
            }).ToList();

            // Agregamos los nuevos a la colección de la entidad trackeada
            foreach(var det in nuevosDetallesEF)
            {
                entity.Detalles.Add(det);
            }

            await _context.SaveChangesAsync();
        }

        // Agregar esta sobrecarga en la interfaz IFacturaRepository y aquí
        public async Task<bool> ExisteNumeroFacturaAsync(short idProveedor, string numero, int? idExcluir = null)
        {
            var query = _context.Facturas.Where(f => f.IdProveedor == idProveedor && f.NumeroFactura == numero);
            
            if (idExcluir.HasValue)
            {
                query = query.Where(f => f.IdFactura != idExcluir.Value);
            }

            return await query.AnyAsync();
        }


        public async Task ActualizarSaldoYEstadoAsync(int idFactura, decimal nuevoSaldo, bool pagada,DateTime FechaP)
        {
            var factura = await _context.Facturas
                .FirstOrDefaultAsync(f => f.IdFactura == idFactura);
                
            if (factura == null) throw new Exception("Factura no encontrada");
            
            factura.Saldo = nuevoSaldo;
            factura.Pagada = pagada;
            factura.FechaPago = FechaP;
            
            await _context.SaveChangesAsync();
        }


    }
}