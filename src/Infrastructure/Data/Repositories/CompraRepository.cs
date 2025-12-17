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
            return _context.Compras.Include(c => c.Proveedor)
            .Include(c => c.Usuario)
            .Include(c => c.Detalles)
            .ThenInclude(i => i.Producto);
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
            await _context.Compras.AddAsync(compraEF);
        }
    }
}