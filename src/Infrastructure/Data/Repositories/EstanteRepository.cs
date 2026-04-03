using Microsoft.EntityFrameworkCore;
using src.Models.Mappers;
using src.Repositories.Interfaces;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace src.Repositories.Implementations
{
    public class EstanteRepository : IEstanteRepository
    {
        private readonly EF.AppDbContext _context;
        private readonly EstanteMapper _estanteMapper;

        public EstanteRepository(EF.AppDbContext context, EstanteMapper estanteMapper)
        {
            _context = context;
            _estanteMapper = estanteMapper;
        }

        public async Task<IEnumerable<Dom.Estante>> GetAllAsync()
        {
            var efEstantes = await _context.Estantes
                .Include(e => e.Deposito)
                    .ThenInclude(de => de.Direccion)
                        .ThenInclude(dom => dom.IdProvinciaNavigation)
                .Include(e => e.Filas)
                .AsNoTracking()
                .ToListAsync();

            return _estanteMapper.ToDomain(efEstantes);
        }

        public async Task<IEnumerable<Dom.Estante>> GetByDepositoAsync(int idDeposito)
        {
            var efEstantes = await _context.Estantes
                .Where(e => e.IdDeposito == idDeposito)
                .Include(e => e.Deposito)
                    .ThenInclude(de => de.Direccion)
                        .ThenInclude(dom => dom.IdProvinciaNavigation)
                .Include(e => e.Filas)
                .AsNoTracking()
                .ToListAsync();

            return _estanteMapper.ToDomain(efEstantes);
        }

        public async Task<Dom.Estante?> GetByIdAsync(int id)
        {
            var efEstante = await _context.Estantes
                .Include(e => e.Deposito)
                    .ThenInclude(de => de.Direccion)
                        .ThenInclude(dom => dom.IdProvinciaNavigation)
                .Include(e => e.Filas)
                    .ThenInclude(f => f.UbicacionesProductos)
                        .ThenInclude(up => up.Producto)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.IdEstante == id);

            return efEstante == null ? null : _estanteMapper.ToDomain(efEstante);
        }

        public async Task AddAsync(Dom.Estante entity)
        {
            // No usar el mapper: el entity que llega es un stub
            // (Deposito solo tiene IdDeposito seteado — el mapper crashea al navegar Deposito.Direccion...)
            var efEstante = new EF.Estante
            {
                IdEstante     = 0,
                NumeroEstante = entity.NumeroEstante,
                Activo        = entity.Activo,
                TieneEspacio  = entity.TieneEspacio,
                Observaciones = entity.Observaciones,
                IdDeposito    = entity.Deposito.IdDeposito   // ← FK escalar, sin navegar
            };

            bool depositoExiste = await _context.Depositos
                .AnyAsync(d => d.IdDeposito == entity.Deposito.IdDeposito && d.Activo);
            if (!depositoExiste)
                throw new InvalidOperationException(
                    $"El depósito {entity.Deposito.IdDeposito} no existe o está inactivo.");

            _context.Estantes.Add(efEstante);
            await _context.SaveChangesAsync();

            entity.IdEstante = efEstante.IdEstante;
        }

        public async Task UpdateAsync(Dom.Estante entity)
        {
            var existing = await _context.Estantes
                .FirstOrDefaultAsync(e => e.IdEstante == entity.IdEstante);

            if (existing == null)
                throw new KeyNotFoundException($"Estante {entity.IdEstante} no encontrado");

            existing.NumeroEstante = entity.NumeroEstante;
            existing.Activo = entity.Activo;
            existing.TieneEspacio = entity.TieneEspacio;
            existing.Observaciones = entity.Observaciones;
            // El IdDeposito (padre) no se cambia en un update; si hay que mover el estante
            // de depósito sería una operación de negocio separada.

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _context.Estantes.FindAsync(id);
            if (existing != null)
            {
                existing.Activo = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ReactivateAsync(int id)
        {
            var entity = await _context.Estantes.FindAsync(id);
            if (entity == null)
                throw new Exception($"No se encontró el estante con ID {id}");

            entity.Activo = true;
            await _context.SaveChangesAsync();
        }


        public async Task MarcarEstanteLlenoAsync(int idEstante, bool tieneEspacio)
        {
            var estante = await _context.Estantes.FindAsync(idEstante);
            if (estante == null)
                throw new KeyNotFoundException($"Estante {idEstante} no encontrado");

            estante.TieneEspacio = tieneEspacio;
            await _context.SaveChangesAsync();
        }

        public async Task SincronizarEspacioAsync(int idEstante)
        {
            var estante = await _context.Estantes
                .Include(e => e.Filas)
                .FirstOrDefaultAsync(e => e.IdEstante == idEstante);

            if (estante is null)
                throw new KeyNotFoundException($"Estante {idEstante} no encontrado.");

            var filasActivas = estante.Filas.Where(f => f.Activo).ToList();

            // Si no hay filas activas → no cambiar el estado (no hay información suficiente)
            if (!filasActivas.Any())
                return;

            // Tiene espacio si al menos una fila activa tiene espacio
            bool tieneEspacio = filasActivas.Any(f => f.TieneEspacio);

            if (estante.TieneEspacio != tieneEspacio)
            {
                estante.TieneEspacio = tieneEspacio;
                await _context.SaveChangesAsync();
            }
        }

        



    }
}