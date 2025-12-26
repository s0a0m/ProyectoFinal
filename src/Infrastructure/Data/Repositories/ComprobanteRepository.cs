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
    public class ComprobanteRepository : IComprobanteRepository
    {
        private readonly EF.AppDbContext _context;
        public ComprobanteRepository(EF.AppDbContext context)
        {
            _context = context;
        }
        private IQueryable<EF.Comprobante> GetQueryComprobante()
        {
            return _context.Comprobantes
                .AsNoTracking()
                .Include(c => c.Proveedor)
                .Include(c => c.Motivo)
                .Include(c => c.CondicionPago);
        }
        private IQueryable<EF.NotaCredito> GetQueryNotaCredito()
        {
            return _context.Comprobantes.OfType<EF.NotaCredito>()
                .AsNoTracking()
                .Include(n => n.Proveedor)
                .Include(n => n.Motivo)
                .Include(n => n.CondicionPago)
                .Include(n => n.Proveedor)
                    .ThenInclude(f => f.IdDomicilioNavigation)
                        .ThenInclude(f => f.IdProvinciaNavigation)
                .Include(n => n.Proveedor)
                    .ThenInclude(f => f.IdCondicionPagoHabitualNavigation)
                .Include(n => n.FacturaOriginal)
                    .ThenInclude(f => f.CondicionPago)
                .Include(n => n.FacturaOriginal)
                    .ThenInclude(f => f.Proveedor)
                        .ThenInclude(p => p.IdCondicionPagoHabitualNavigation)
                .Include(n => n.FacturaOriginal)
                    .ThenInclude(f => f.Proveedor)
                        .ThenInclude(p => p.IdDomicilioNavigation)
                            .ThenInclude(p => p.IdProvinciaNavigation);
        }

        private IQueryable<EF.NotaDebito> GetQueryNotaDebito()
        {
            return _context.Comprobantes.OfType<EF.NotaDebito>()
                .AsNoTracking()
                .Include(n => n.Proveedor)
                .Include(n => n.Motivo)
                .Include(n => n.CondicionPago)
                .Include(n => n.Proveedor)
                    .ThenInclude(f => f.IdDomicilioNavigation)
                        .ThenInclude(f => f.IdProvinciaNavigation)
                .Include(n => n.Proveedor)
                    .ThenInclude(f => f.IdCondicionPagoHabitualNavigation)
                .Include(n => n.FacturaOriginal)
                    .ThenInclude(f => f.CondicionPago)
                .Include(n => n.FacturaOriginal)
                    .ThenInclude(f => f.Proveedor)
                        .ThenInclude(p => p.IdCondicionPagoHabitualNavigation)
                .Include(n => n.FacturaOriginal)
                    .ThenInclude(f => f.Proveedor)
                        .ThenInclude(p => p.IdDomicilioNavigation)
                            .ThenInclude(p => p.IdProvinciaNavigation);
        }

        public async Task<Dom.Comprobante?> GetByIdAsync(int id)
        {
            var nc = await GetQueryNotaCredito().FirstOrDefaultAsync(c => c.IdComprobante == id);

            if (nc != null)
            {
                return DominioMapper.Map(nc);
            }

            var nd = await GetQueryNotaDebito()
                .FirstOrDefaultAsync(c => c.IdComprobante == id);

            if (nd != null)
            {
                return DominioMapper.Map(nd);
            }

            var comp = await GetQueryComprobante()
                .FirstOrDefaultAsync(c => c.IdComprobante == id);

            return comp != null ? DominioMapper.Map(comp) : null;
        }
        public async Task<IEnumerable<NotaCredito>> GetNotasCreditoAsync()
        {
            var list = await GetQueryNotaCredito()
            .OrderByDescending(x => x.FechaEmision)
            .ToListAsync();

            return DominioMapper.Map(list);
        }

        public async Task<IEnumerable<NotaDebito>> GetNotasDebitoAsync()
        {
            var list = await GetQueryNotaDebito()
                .OrderByDescending(x => x.FechaEmision)
                .ToListAsync();

            return DominioMapper.Map(list);
        }

        public async Task<IEnumerable<MotivoComprobante>> GetMotivosAsync(string? claseComprobante = null)
        {
            var query = _context.MotivosComprobante.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(claseComprobante))
            {
                string tipoNormalizado = claseComprobante.ToUpper().Trim();
                query = query.Where(m =>
                    m.ClaseComprobante == tipoNormalizado ||
                    m.ClaseComprobante == "TODOS");
            }

            var listaEf = await query.ToListAsync();
            return DominioMapper.Map(listaEf);
        }

        public async Task<Dom.Comprobante> AddAsync(Dom.Comprobante comprobanteDom)
        {
            var entityEf = DominioMapper.Map(comprobanteDom);
            _context.Comprobantes.Add(entityEf);
            await _context.SaveChangesAsync();
            comprobanteDom.IdComprobante = entityEf.IdComprobante;

            return comprobanteDom;
        }
        public Task UpdateAsync(Comprobante comprobante)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Comprobante>> GetByFacturaIdAsync(int idFactura)
        {
            var comprobantesEf = await GetQueryComprobante()
                .Where(c => c.IdFacturaReferencia == idFactura)
                .OrderByDescending(c => c.FechaEmision)
                .ToListAsync();

            return DominioMapper.Map(comprobantesEf);
        }

        public async Task<IEnumerable<Comprobante>> GetByProveedorAsync(short idProveedor)
        {
            var ComprobanteEF = await GetQueryComprobante()
                .AsNoTracking()
                .Where(f => f.IdProveedor == idProveedor)
                .ToListAsync();
            return DominioMapper.Map(ComprobanteEF);
        }


    }
}