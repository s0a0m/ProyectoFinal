using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using src.Models.Common;
using src.Models.Domain;
using src.Models.Mappers;
using src.Repositories.Interfaces;
using src.Repositories.Interfaces;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;

namespace src.Repositories.Implementations
{
    public class ComprobanteRepository : IComprobanteRepository
    {
        private readonly EF.AppDbContext _context;
        private readonly ComprobanteMapper _comprobanteMapper;

        public ComprobanteRepository(EF.AppDbContext context, ComprobanteMapper comprobanteMapper)
        {
            _context = context;
            _comprobanteMapper = comprobanteMapper;
        }

        private IQueryable<EF.Comprobante> GetQueryComprobante()
        {
            return _context
                .Comprobantes.AsNoTracking()
                .Include(n => n.Proveedor)
                .Include(n => n.Motivo)
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

        private IQueryable<EF.NotaCredito> GetQueryNotaCredito()
        {
            return _context
                .Comprobantes.OfType<EF.NotaCredito>()
                .AsNoTracking()
                .Include(n => n.Proveedor)
                .Include(n => n.Motivo)
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
            return _context
                .Comprobantes.OfType<EF.NotaDebito>()
                .AsNoTracking()
                .Include(n => n.Proveedor)
                .Include(n => n.Motivo)
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
                return _comprobanteMapper.ToDomain(nc);
            }

            var nd = await GetQueryNotaDebito().FirstOrDefaultAsync(c => c.IdComprobante == id);

            if (nd != null)
            {
                return _comprobanteMapper.ToDomain(nd);
            }

            var comp = await GetQueryComprobante().FirstOrDefaultAsync(c => c.IdComprobante == id);

            return comp != null ? _comprobanteMapper.ToDomain(comp) : null;
        }

        public async Task<IEnumerable<NotaCredito>> GetNotasCreditoAsync()
        {
            var list = await GetQueryNotaCredito()
                .OrderByDescending(x => x.FechaEmision)
                .ToListAsync();

            return _comprobanteMapper.ToDomain(list);
        }

        public async Task<IEnumerable<NotaDebito>> GetNotasDebitoAsync()
        {
            var list = await GetQueryNotaDebito()
                .OrderByDescending(x => x.FechaEmision)
                .ToListAsync();

            return _comprobanteMapper.ToDomain(list);
        }

        public async Task<IEnumerable<MotivoComprobante>> GetMotivosAsync(
            string? claseComprobante = null
        )
        {
            var query = _context.MotivosComprobante.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(claseComprobante))
            {
                string tipoNormalizado = claseComprobante.ToUpper().Trim();
                query = query.Where(m =>
                    m.ClaseComprobante == tipoNormalizado || m.ClaseComprobante == "TODOS"
                );
            }

            var listaEf = await query.ToListAsync();
            return DominioMapper.Map(listaEf);
        }

        public async Task<Dom.Comprobante> AddAsync(Dom.Comprobante comprobanteDom)
        {
            var entityEf = _comprobanteMapper.ToEntity(comprobanteDom);

            entityEf.IdProveedor = (short)comprobanteDom.Proveedor!.IdProveedor;
            entityEf.IdFacturaReferencia = comprobanteDom.IdFacturaReferencia!.Value;
            entityEf.IdMotivo = (short)comprobanteDom.Motivo!.IdMotivo;

            entityEf.Proveedor = null!;
            entityEf.FacturaOriginal = null!;
            entityEf.Motivo = null!;

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

            return _comprobanteMapper.ToDomain(comprobantesEf);
        }

        public async Task<IEnumerable<Comprobante>> GetByProveedorAsync(short idProveedor)
        {
            var ComprobanteEF = await GetQueryComprobante()
                .AsNoTracking()
                .Where(f => f.IdProveedor == idProveedor)
                .ToListAsync();
            return _comprobanteMapper.ToDomain(ComprobanteEF);
        }
    }
}
