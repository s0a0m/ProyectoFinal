using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using src.Repositories.Interfaces;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;
using src.Models.Mappers;

namespace src.Repositories.Implementations
{
    public class FamiliaRepository : IFamiliaRepository
    {
        private readonly EF.AppDbContext _context;
        private readonly FamiliaMapper _familiaMapper;

        public FamiliaRepository(EF.AppDbContext context, FamiliaMapper familiaMapper)
        {
            _context = context;
            _familiaMapper = familiaMapper;
        }

        public async Task<IEnumerable<Dom.Familia>> GetAllAsync()
        {
            var data = await _context.Familias.AsNoTracking().ToListAsync();
            return _familiaMapper.ToDomain(data);
        }

        public async Task<IEnumerable<Dom.Familia>> GetAllWithCategoriasAsync()
        {

            var data = await _context.Familias
                .Include(f => f.Categorias)
                .AsNoTracking()
                .ToListAsync();

            return _familiaMapper.ToDomain(data);
        }

        public async Task<Dom.Familia?> GetByIdAsync(short id)
        {
            var data = await _context.Familias
                .Include(f => f.Categorias)
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.IdFamilia == id);

            return data == null ? null : _familiaMapper.ToDomain(data);
        }

        public async Task AddAsync(Dom.Familia entity)
        {
            var efEntity = _familiaMapper.ToEntity(entity);
            efEntity.IdFamilia = 0; // Asegurar identidad

            _context.Familias.Add(efEntity);
            await _context.SaveChangesAsync();

            entity.IdFamilia = efEntity.IdFamilia;
        }

        public async Task UpdateAsync(Dom.Familia entity)
        {
            var existing = await _context.Familias.FindAsync(entity.IdFamilia);
            if (existing == null) throw new KeyNotFoundException($"Familia {entity.IdFamilia} no encontrada");

            var efEntity = _familiaMapper.ToEntity(entity);
            _context.Entry(existing).CurrentValues.SetValues(efEntity);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasCategoriasAsync(short idFamilia)
        {
            return await _context.Categorias.AnyAsync(c => c.IdFamilia == idFamilia);
        }

        public async Task DeleteAsync(short id)
        {
            var existing = await _context.Familias.FindAsync(id);
            if (existing != null)
            {
                _context.Familias.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsNombreAsync(string nombre, short? idExcluir = null)
        {
            var nombreNormalizado = nombre.Trim().ToLower();

            var query = _context.Familias.AsQueryable();

            if (idExcluir.HasValue)
            {
                query = query.Where(f => f.IdFamilia != idExcluir.Value);
            }

            return await query.AnyAsync(f => f.Nombre.ToLower() == nombreNormalizado);
        }
    }
}