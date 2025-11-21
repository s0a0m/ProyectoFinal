using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using src.Repositories.Interfaces;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;
using src.Models.Mappers;

namespace src.Repositories.Implementations
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly EF.AppDbContext _context;

        public CategoriaRepository(EF.AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Dom.Categoria>> GetAllAsync()
        {
            // 1. Traemos los datos de EF con la Familia incluida
            var efCategorias = await _context.Categorias
                .Include(c => c.Familia) 
                .AsNoTracking()
                .ToListAsync();

            // 2. Mapeo MANUAL (Bypaseamos a DominioMapper aquí)
            //    Esto nos garantiza que la Familia se cargue sí o sí.
            var categoriasDominio = efCategorias.Select(efCat => new Dom.Categoria
            {
                IdCategoria = efCat.IdCategoria,
                Nombre = efCat.Nombre,
                Descripcion = efCat.Descripcion,
                
                // Aquí construimos la Familia manualmente si existe
                Familia = efCat.Familia == null ? null : new Dom.Familia 
                {
                    IdFamilia = efCat.Familia.IdFamilia,
                    Nombre = efCat.Familia.Nombre,
                    Descripcion = efCat.Familia.Descripcion
                    // IMPORTANTE: No mapeamos la lista 'Categorias' dentro de Familia 
                    // para evitar el bucle infinito. La dejamos vacía.
                }
            }).ToList();

            return categoriasDominio;
        }

        public async Task<IEnumerable<Dom.Categoria>> GetByFamiliaIdAsync(short idFamilia)
        {
            var data = await _context.Categorias
                .Where(c => c.IdFamilia == idFamilia)
                .AsNoTracking()
                .ToListAsync();
            return DominioMapper.Map(data);
        }

        public async Task<Dom.Categoria?> GetByIdAsync(short id)
        {
            var data = await _context.Categorias
                .Include(c => c.Familia)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdCategoria == id);

            return data == null ? null : DominioMapper.Map(data);
        }

        public async Task AddAsync(Dom.Categoria entity)
        {
            var efEntity = DominioMapper.Map(entity);
            efEntity.IdCategoria = 0;

            // Nota: Aseguramos que el IdFamilia esté seteado correctamente.
            // Tu mapper usa 'Dom.Categoria.Familia.IdFamilia' -> 'EF.Categoria.IdFamilia'.
            // El servicio deberá asegurarse de que entity.Familia no sea null y tenga el ID.
            
            _context.Categorias.Add(efEntity);
            await _context.SaveChangesAsync();
            
            // No podemos asignar de vuelta a una propiedad 'init', 
            // pero en C# 9+ con 'init' puedes hacerlo en la construcción. 
            // Si necesitas el ID de vuelta, tu Dom.Categoria debería tener set público o interno.
        }

        public async Task UpdateAsync(Dom.Categoria entity)
        {
            var existing = await _context.Categorias.FindAsync(entity.IdCategoria);
            if (existing == null) throw new KeyNotFoundException($"Categoría {entity.IdCategoria} no encontrada");

            var efEntity = DominioMapper.Map(entity);
            
            // Actualizamos valores escalares y FKs
            _context.Entry(existing).CurrentValues.SetValues(efEntity);
            
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasProductosAsync(short idCategoria)
        {
            return await _context.ProductoCategorias.AnyAsync(pc => pc.IdCategoria == idCategoria);
        }

        public async Task DeleteAsync(short id)
        {
            var existing = await _context.Categorias.FindAsync(id);
            if (existing != null)
            {
                _context.Categorias.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsNombreEnFamiliaAsync(string nombre, short idFamilia, short? idExcluir = null)
        {
            var nombreNormalizado = nombre.Trim().ToLower();

            // Filtramos primero por familia
            var query = _context.Categorias
                .Where(c => c.IdFamilia == idFamilia);

            // Si es update, excluimos el ID actual
            if (idExcluir.HasValue)
            {
                query = query.Where(c => c.IdCategoria != idExcluir.Value);
            }

            return await query.AnyAsync(c => c.Nombre.ToLower() == nombreNormalizado);
        }

                
    }
}