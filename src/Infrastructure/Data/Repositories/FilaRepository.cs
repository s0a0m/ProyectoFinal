using Microsoft.EntityFrameworkCore;
using src.Models.Mappers;
using src.Repositories.Interfaces;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace src.Repositories.Implementations
{
    public class FilaRepository : IFilaRepository
    {
        private readonly EF.AppDbContext _context;
        private readonly FilaMapper _filaMapper;

        public FilaRepository(EF.AppDbContext context, FilaMapper filaMapper)
        {
            _context = context;
            _filaMapper = filaMapper;
        }

        public async Task<IEnumerable<Dom.Fila>> GetAllAsync()
        {
            var efFilas = await _context.Filas
                .Include(f => f.Estante)
                    .ThenInclude(e => e.Deposito)
                        .ThenInclude(de => de.Direccion)
                            .ThenInclude(dom => dom.IdProvinciaNavigation)
                
                .AsNoTracking()
                .ToListAsync();

            return _filaMapper.ToDomain(efFilas);
        }

        public async Task<IEnumerable<Dom.Fila>> GetByEstanteAsync(int idEstante)
        {
            var efFilas = await _context.Filas
                .Where(f => f.IdEstante == idEstante)
                .Include(f => f.Estante)
                    .ThenInclude(e => e.Deposito)
                        .ThenInclude(de => de.Direccion)
                            .ThenInclude(dom => dom.IdProvinciaNavigation)
                .Include(f => f.UbicacionesProductos)
                    .ThenInclude(up => up.Producto)
                .AsNoTracking()
                .ToListAsync();

            return _filaMapper.ToDomain(efFilas);
        }

        public async Task<Dom.Fila?> GetByIdAsync(int id)
        {
            var efFila = await _context.Filas
                .Include(f => f.Estante)
                    .ThenInclude(e => e.Deposito)
                        .ThenInclude(de => de.Direccion)
                            .ThenInclude(dom => dom.IdProvinciaNavigation)
                .Include(f => f.UbicacionesProductos)
                    .ThenInclude(up => up.Producto)
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.IdFila == id);

            return efFila == null ? null : _filaMapper.ToDomain(efFila);
        }

        public async Task AddAsync(Dom.Fila entity)
        {
            // Mismo motivo: Estante es un stub, el mapper navegaría Estante.Deposito.Direccion... y crashea
            var efFila = new EF.Fila
            {
                IdFila        = 0,
                NFila         = entity.NFila,
                Activo        = entity.Activo,
                TieneEspacio  = entity.TieneEspacio,
                Observaciones = entity.Observaciones,
                IdEstante     = entity.Estante.IdEstante     // ← FK escalar, sin navegar
            };

            bool estanteExiste = await _context.Estantes
                .AnyAsync(e => e.IdEstante == entity.Estante.IdEstante && e.Activo);
            if (!estanteExiste)
                throw new InvalidOperationException(
                    $"El estante {entity.Estante.IdEstante} no existe o está inactivo.");

            _context.Filas.Add(efFila);
            await _context.SaveChangesAsync();

            entity.IdFila = efFila.IdFila;
        }

        public async Task UpdateAsync(Dom.Fila entity)
        {
            var existing = await _context.Filas
                .FirstOrDefaultAsync(f => f.IdFila == entity.IdFila);

            if (existing == null)
                throw new KeyNotFoundException($"Fila {entity.IdFila} no encontrada");

            existing.NFila = entity.NFila;
            existing.Activo = entity.Activo;
            existing.TieneEspacio = entity.TieneEspacio;
            existing.Observaciones = entity.Observaciones;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _context.Filas.FindAsync(id);
            if (existing != null)
            {
                existing.Activo = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ReactivateAsync(int id)
        {
            var entity = await _context.Filas.FindAsync(id);
            if (entity == null)
                throw new Exception($"No se encontró la fila con ID {id}");

            entity.Activo = true;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Devuelve las filas con espacio disponible y que esten activas dentro de un depósito específico.
        /// Útil al momento de asignar una nueva ubicación a un producto.
        /// </summary>
        public async Task<IEnumerable<Dom.Fila>> GetFilasConEspacioByDepositoAsync(int idDeposito)
        {
            var efFilas = await _context.Filas
                .Where(f => f.Activo && f.TieneEspacio && f.Estante.IdDeposito == idDeposito)
                .Include(f => f.Estante)
                    .ThenInclude(e => e.Deposito)
                .AsNoTracking()
                .ToListAsync();

            return _filaMapper.ToDomain(efFilas);
        }
    }
}