using Microsoft.EntityFrameworkCore;
using src.Models.Mappers;
using src.Repositories.Interfaces;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace src.Repositories.Implementations
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly EF.AppDbContext _context;

        public ProductoRepository(EF.AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Dom.Producto>> GetAllAsync()
        {
            // Traemos todo, incluyendo relaciones para mostrar en la tabla
            var efProductos = await _context.Productos
                .Include(p => p.ProductoCodigoBarras)
                    .ThenInclude(pcb => pcb.CodigoBarra)
                .Include(p => p.ProductosCategorias)
                    .ThenInclude(pc => pc.Categoria)
                .AsNoTracking()
                .ToListAsync();

            return DominioMapper.Map(efProductos);
        }

        public async Task<Dom.Producto?> GetByIdAsync(int id)
        {
            var efProducto = await _context.Productos
                .Include(p => p.ProductoCodigoBarras)
                    .ThenInclude(pcb => pcb.CodigoBarra)
                .Include(p => p.ProductosCategorias)
                    .ThenInclude(pc => pc.Categoria)
                    .ThenInclude(c => c.Familia) // Útil si necesitas mostrar la familia
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdProducto == id);

            return efProducto == null ? null : DominioMapper.Map(efProducto);
        }

        public async Task AddAsync(Dom.Producto entity)
        {
            // 1. Mapear entidad base
            var efProducto = DominioMapper.Map(entity);
            efProducto.IdProducto = 0; // Asegurar identidad

            // 2. Manejo de Relaciones N-N (Códigos de Barra)
            // El mapper ya creó los objetos EF.ProductoCodigoBarra, pero necesitamos
            // asegurarnos de que los CodigoBarra apunten a existentes o nuevos correctamente.
            
            // Limpiamos la colección generada por el mapper para rellenarla manualmente y con control
            var codigosDelDominio = entity.CodigoBarra.Select(c => c.Codigo).ToList();
            efProducto.ProductoCodigoBarras.Clear();

            foreach (var codigoValor in codigosDelDominio)
            {
                // Buscar si el código ya existe en la tabla maestra
                var codigoMaestro = await _context.CodigoBarras
                    .FirstOrDefaultAsync(cb => cb.Codigo == codigoValor);

                if (codigoMaestro == null)
                {
                    codigoMaestro = new EF.CodigoBarra { Codigo = codigoValor };
                    // No lo agregamos al contexto todavía, EF lo hará al guardar el producto
                }

                efProducto.ProductoCodigoBarras.Add(new EF.ProductoCodigoBarra
                {
                    CodigoBarra = codigoMaestro
                });
            }

            // 3. Manejo de Relaciones N-N (Categorías)
            // Similar a códigos, aseguramos que solo enlazamos por ID
            var idsCategorias = entity.Categoria.Select(c => c.IdCategoria).ToList();
            efProducto.ProductosCategorias.Clear();
            
            foreach (var idCat in idsCategorias)
            {
                efProducto.ProductosCategorias.Add(new EF.ProductoCategoria
                {
                    IdCategoria = (short)idCat
                });
            }

            _context.Productos.Add(efProducto);
            await _context.SaveChangesAsync();
            
            entity.IdProducto = efProducto.IdProducto;
        }

        public async Task UpdateAsync(Dom.Producto entity)
        {
            var existing = await _context.Productos
                .Include(p => p.ProductosCategorias)
                .Include(p => p.ProductoCodigoBarras)
                    .ThenInclude(pcb => pcb.CodigoBarra)
                .FirstOrDefaultAsync(p => p.IdProducto == entity.IdProducto);

            if (existing == null) throw new KeyNotFoundException($"Producto {entity.IdProducto} no encontrado");

            // 1. Actualizar Escalares
            existing.Nombre = entity.Nombre;
            existing.StockMinimo = entity.StockMinimo;
            existing.StockTotal = entity.StockTotal;
            existing.Activo = entity.Activo;

            // 2. Sincronizar Categorías (N-N) - (Tu lógica aquí estaba bien, la mantengo)
            var nuevosIdsCategorias = entity.Categoria.Select(c => (short)c.IdCategoria).ToList();
            foreach (var existingLink in existing.ProductosCategorias.ToList())
            {
                if (!nuevosIdsCategorias.Contains(existingLink.IdCategoria))
                    _context.ProductoCategorias.Remove(existingLink);
            }
            foreach (var newId in nuevosIdsCategorias)
            {
                if (!existing.ProductosCategorias.Any(pc => pc.IdCategoria == newId))
                    existing.ProductosCategorias.Add(new EF.ProductoCategoria { IdProducto = existing.IdProducto, IdCategoria = newId });
            }

            // 3. Sincronizar Códigos de Barra (CORREGIDO)
            var nuevosCodigos = entity.CodigoBarra.Select(c => c.Codigo).ToList();

            // A. Eliminar relaciones viejas
            foreach (var existingLink in existing.ProductoCodigoBarras.ToList())
            {
                if (!nuevosCodigos.Contains(existingLink.CodigoBarra.Codigo))
                {
                    _context.ProductoCodigosBarras.Remove(existingLink);
                }
            }

            // B. Agregar nuevos
            foreach (var codigoValor in nuevosCodigos)
            {
                // Verificamos si ya está relacionado en memoria o en BD
                bool yaRelacionado = existing.ProductoCodigoBarras
                    .Any(pcb => pcb.CodigoBarra.Codigo == codigoValor);

                if (!yaRelacionado)
                {
                    // 1. Buscar en la tabla maestra
                    var codigoMaestro = await _context.CodigoBarras
                        .FirstOrDefaultAsync(cb => cb.Codigo == codigoValor);
                    
                    if (codigoMaestro == null)
                    {
                        // 2. CASO CÓDIGO NUEVO: Lo creamos y lo agregamos EXPLÍCITAMENTE al contexto
                        codigoMaestro = new EF.CodigoBarra { Codigo = codigoValor };
                        _context.CodigoBarras.Add(codigoMaestro); 
                        // Al hacer Add, su estado pasa a 'Added'.
                    }

                    // 3. Crear el enlace
                    // Nota: Asignamos explícitamente el IdProducto para asegurar la integridad
                    var nuevaRelacion = new EF.ProductoCodigoBarra
                    {
                        IdProducto = existing.IdProducto, // Asegurar enlace con el padre
                        CodigoBarra = codigoMaestro       // Enlace con el hijo (nuevo o existente)
                    };

                    existing.ProductoCodigoBarras.Add(nuevaRelacion);
                }
            }

            await _context.SaveChangesAsync();
        }
       /* public async Task UpdateAsync(Dom.Producto entity)
        {
            var existing = await _context.Productos
                .Include(p => p.ProductosCategorias)
                .Include(p => p.ProductoCodigoBarras)
                    .ThenInclude(pcb => pcb.CodigoBarra)
                .FirstOrDefaultAsync(p => p.IdProducto == entity.IdProducto);

            if (existing == null) throw new KeyNotFoundException($"Producto {entity.IdProducto} no encontrado");

            // 1. Actualizar Escalares
            existing.Nombre = entity.Nombre;
            existing.StockMinimo = entity.StockMinimo;
            existing.StockTotal = entity.StockTotal;
            existing.Activo = entity.Activo;

            // 2. Sincronizar Categorías (N-N)
            var nuevosIdsCategorias = entity.Categoria.Select(c => (short)c.IdCategoria).ToList();
            
            // A. Eliminar
            foreach (var existingLink in existing.ProductosCategorias.ToList())
            {
                if (!nuevosIdsCategorias.Contains(existingLink.IdCategoria))
                {
                    _context.ProductoCategorias.Remove(existingLink);
                }
            }
            // B. Agregar
            foreach (var newId in nuevosIdsCategorias)
            {
                if (!existing.ProductosCategorias.Any(pc => pc.IdCategoria == newId))
                {
                    existing.ProductosCategorias.Add(new EF.ProductoCategoria 
                    { 
                        IdProducto = existing.IdProducto, 
                        IdCategoria = newId
                    });
                }
            }

            // 3. Sincronizar Códigos de Barra (N-N)
            var nuevosCodigos = entity.CodigoBarra.Select(c => c.Codigo).ToList();

            // A. Eliminar relaciones
            foreach (var existingLink in existing.ProductoCodigoBarras.ToList())
            {
                // Comparamos por el string del código
                if (!nuevosCodigos.Contains(existingLink.CodigoBarra.Codigo))
                {
                    _context.ProductoCodigosBarras.Remove(existingLink);
                    
                    // Opcional: Si quieres borrar el código de la tabla maestra si ya no se usa
                    // requeriría una verificación adicional (count == 0). Por ahora lo dejamos.
                }
            }

            // B. Agregar nuevos
            foreach (var codigoValor in nuevosCodigos)
            {
                bool yaRelacionado = existing.ProductoCodigoBarras.Any(pcb => pcb.CodigoBarra.Codigo == codigoValor);

                if (!yaRelacionado)
                {
                    var codigoMaestro = await _context.CodigoBarras.FirstOrDefaultAsync(cb => cb.Codigo == codigoValor);
                    
                    if (codigoMaestro == null)
                    {
                        codigoMaestro = new EF.CodigoBarra { Codigo = codigoValor };
                        // Al agregarlo a la colección de navegación, EF lo insertará
                    }

                    existing.ProductoCodigoBarras.Add(new EF.ProductoCodigoBarra
                    {
                        CodigoBarra = codigoMaestro
                    });
                }
            }

            await _context.SaveChangesAsync();
        }
        */
        public async Task DeleteAsync(int id)
        {
            var existing = await _context.Productos.FindAsync((short)id);
            if (existing != null)
            {
                existing.Activo = false; // Baja Lógica
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsCodigoBarraAsync(string codigo, int? excluirProductoId = null)
        {
            var query = _context.ProductoCodigosBarras
                .Include(pcb => pcb.CodigoBarra)
                .Where(pcb => pcb.CodigoBarra.Codigo == codigo);

            if (excluirProductoId.HasValue)
            {
                query = query.Where(pcb => pcb.IdProducto != excluirProductoId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task ReactivateAsync(int id)
        {
            var entity = await _context.Productos.FindAsync((short)id);
            if (entity != null)
            {
                entity.Activo = true; 
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            else 
            {
                throw new Exception($"No se encontró el producto con ID {id}");
            }
        }
    }
}