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

        public FamiliaRepository(EF.AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Dom.Familia>> GetAllAsync()
        {
            var data = await _context.Familias.AsNoTracking().ToListAsync();
            return DominioMapper.Map(data);
        }

        public async Task<IEnumerable<Dom.Familia>> GetAllWithCategoriasAsync()
        {
            
            var data = await _context.Familias
                .Include(f => f.Categorias)
                .AsNoTracking()
                .ToListAsync();

            return DominioMapper.Map(data);
        }

        public async Task<Dom.Familia?> GetByIdAsync(short id)
        {
            var data = await _context.Familias
                .Include(f => f.Categorias) 
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.IdFamilia == id);

            return data == null ? null : DominioMapper.Map(data);
        }

        public async Task AddAsync(Dom.Familia entity)
        {
            var efEntity = DominioMapper.Map(entity);
            efEntity.IdFamilia = 0; // Asegurar identidad
            
            _context.Familias.Add(efEntity);
            await _context.SaveChangesAsync();
            
            entity.IdFamilia = efEntity.IdFamilia;
        }

        public async Task UpdateAsync(Dom.Familia entity)
        {
            var existing = await _context.Familias.FindAsync(entity.IdFamilia);
            if (existing == null) throw new KeyNotFoundException($"Familia {entity.IdFamilia} no encontrada");

            var efEntity = DominioMapper.Map(entity);
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
    }
}