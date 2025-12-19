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
    public class CompraRepository : ICompraRepository
    {
        private readonly EF.AppDbContext _context;

        public CompraRepository(EF.AppDbContext context)
        {
            _context = context;
        }
        private IQueryable<EF.Compra> GetQueryCompras()
        {
            return _context.Compras
            .Include(c => c.Usuario)
            .Include(c => c.Detalles)
            .ThenInclude(i => i.Producto)
            .Include(c => c.Proveedor)
            .ThenInclude(p => p.IdCondicionPagoHabitualNavigation)
            .Include(c => c.Proveedor)
            .ThenInclude(p => p.IdDomicilioNavigation)
            .ThenInclude(d => d.IdProvinciaNavigation);
        }
        public async Task<IEnumerable<Compra>> GetAllAsync()
        {
            var comprasEF = await GetQueryCompras().AsNoTracking().OrderByDescending(c => c.FechaCompra).ToListAsync();
            return DominioMapper.Map(comprasEF);
        }
        public async Task<Compra?> GetByIdAsync(int id)
        {
            var compraEF = await GetQueryCompras().AsNoTracking().FirstOrDefaultAsync(c => c.IdCompra == id);
            return compraEF == null ? null : DominioMapper.Map(compraEF);
        }

        public async Task AddAsync(Compra compra)
        {
            var compraEF = DominioMapper.Map(compra);
            compraEF.IdCompra = 0;
            compraEF.IdProveedor = (short)compra.Proveedor.IdProveedor;
            for (int i = 0; i < compra.Detalles.Count; i++)
            {
                var detalleDom = compra.Detalles[i];
                var detalleEF = compraEF.Detalles.ElementAt(i);

                detalleEF.IdDetalleCompra = 0;
                detalleEF.IdProducto = (short)detalleDom.Producto.IdProducto;
            }
            compraEF.IdUsuario = compra.Usuario.IdUsuario;
            await _context.Compras.AddAsync(compraEF);
            await _context.SaveChangesAsync();
            compra.IdCompra = compraEF.IdCompra;
        }

        public async Task UpdateAsync(Compra compra)
        {
            var compraEF = await GetQueryCompras().FirstOrDefaultAsync(c => c.IdCompra == compra.IdCompra);

            if (compraEF == null) return;
            compraEF.Observaciones = compra.Observaciones;


            var detallesEliminar = compraEF.Detalles.Where(d => !compra.Detalles.Any(dd => dd.IdDetalleCompra == d.IdDetalleCompra)).ToList();
            if (detallesEliminar.Any()) _context.DetallesCompra.RemoveRange(detallesEliminar);

            foreach (var detDom in compra.Detalles)
            {
                var detEf = compraEF.Detalles.FirstOrDefault(d => d.IdDetalleCompra == detDom.IdDetalleCompra);

                if (detEf != null)
                {
                    detEf.Cantidad = detDom.Cantidad;
                    detEf.PrecioPactado = Math.Round(detDom.PrecioPactado, 2);
                }
                else
                {
                    compraEF.Detalles.Add(new EF.DetalleCompra
                    {
                        IdProducto = (short)detDom.Producto.IdProducto,
                        Cantidad = detDom.Cantidad,
                        PrecioPactado = Math.Round(detDom.PrecioPactado, 2)
                    });
                }
            }
            await _context.SaveChangesAsync();
        }
        public async Task FinalizarCompraAsync(int idCompra)
        {
            var compra = await _context.Compras.FindAsync((short)idCompra);
            if (compra == null) return;

            compra.Estado = EstadoCompra.COMPLETADA;
            compra.FechaRecepcion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
        public async Task CancelarCompraAsync(int idCompra, string motivo)
        {
            var compra = await _context.Compras.FindAsync((short)idCompra);
            if (compra == null) return;

            compra.Estado = EstadoCompra.CANCELADA;
            compra.Observaciones = $"CANCELADO: {motivo}";

            await _context.SaveChangesAsync();
        }
        public async Task MarcarComoEnviadaAsync(int idCompra)
        {
            var compra = await _context.Compras.FindAsync((short)idCompra);
            if (compra != null)
            {
                compra.Estado = EstadoCompra.ENVIADA;
                await _context.SaveChangesAsync();
            }
        }
    }
}