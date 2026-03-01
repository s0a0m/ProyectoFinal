using Microsoft.EntityFrameworkCore;
using src.Models.Mappers;
using src.Repositories.Interfaces;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace src.Repositories.Implementations
{
    public class DepositoRepository : IDepositoRepository
    {
        private readonly EF.AppDbContext _context;
        private readonly DepositoMapper _depositoMapper;
        private readonly DomicilioMapper _domicilioMapper;

        public DepositoRepository(EF.AppDbContext context, DepositoMapper depositoMapper, DomicilioMapper domicilioMapper)
        {
            _context = context;
            _depositoMapper = depositoMapper;
            _domicilioMapper = domicilioMapper;
        }

        public async Task<IEnumerable<Dom.Deposito>> GetAllAsync()
        {
            var efDepositos = await _context.Depositos
                .Include(d => d.Estantes)
                    .ThenInclude(e => e.Filas)
                .AsNoTracking()
                .ToListAsync();

            return _depositoMapper.ToDomain(efDepositos);
        }

        public async Task<Dom.Deposito?> GetByIdAsync(int id)
        {
            var efDeposito = await _context.Depositos
                .Include(d => d.Estantes)
                    .ThenInclude(e => e.Filas)
                        .ThenInclude(f => f.UbicacionesProductos)
                            .ThenInclude(up => up.Producto)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.IdDeposito == id);

            return efDeposito == null ? null : _depositoMapper.ToDomain(efDeposito);
        }

        public async Task AddAsync(Dom.Deposito entity)
        {
            var efDeposito = _depositoMapper.ToEntity(entity);
            efDeposito.IdDeposito = 0;

            _context.Depositos.Add(efDeposito);
            await _context.SaveChangesAsync();

            entity.IdDeposito = efDeposito.IdDeposito;
        }

        public async Task UpdateAsync(Dom.Deposito entity)
        {
            // Sin AsNoTracking para que EF trackee el Domicilio también
            var existing = await _context.Depositos
                .Include(d => d.Direccion)
                    .ThenInclude(dom => dom.IdProvinciaNavigation)
                .FirstOrDefaultAsync(d => d.IdDeposito == entity.IdDeposito);

            if (existing == null)
                throw new KeyNotFoundException($"Depósito {entity.IdDeposito} no encontrado");

            // 1. Escalares del Depósito
            existing.Nombre = entity.Nombre;
            existing.Activo = entity.Activo;

            // 2. Actualizar Domicilio sobre la entidad trackeada (mismo patrón que Proveedor)
            if (existing.Direccion != null && entity.Direccion != null)
            {
                // Mapeamos el domain Direccion -> EF Domicilio temporal (solo para leer los valores)
                EF.Domicilio domicilioTemporal = _domicilioMapper.ToEntity(entity.Direccion);

                // Pisamos los valores sobre el objeto ya trackeado sin romper la referencia
                _context.Entry(existing.Direccion).CurrentValues.SetValues(domicilioTemporal);

                // FK de Provincia explícita (SetValues no siempre propaga FKs de navegaciones)
                if (entity.Direccion.Prov != null)
                    existing.Direccion.IdProvincia = entity.Direccion.Prov.IdProvincia;
            }
            else if (entity.Direccion != null)
            {
                // Caso borde: el depósito no tenía domicilio y ahora se le asigna uno nuevo
                EF.Domicilio nuevoDomicilio = _domicilioMapper.ToEntity(entity.Direccion);
                nuevoDomicilio.IdDomicilio = 0; // EF lo tratará como INSERT
                _context.Add(nuevoDomicilio);
                existing.Direccion = nuevoDomicilio; // EF resolverá el FK automáticamente
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _context.Depositos.FindAsync(id);
            if (existing != null)
            {
                existing.Activo = false; // Baja lógica
                await _context.SaveChangesAsync();
            }
        }

        public async Task ReactivateAsync(int id)
        {
            var entity = await _context.Depositos.FindAsync(id);
            if (entity == null)
                throw new Exception($"No se encontró el depósito con ID {id}");

            entity.Activo = true;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Devuelve solo los depósitos activos. Útil para combos/selects en la UI.
        /// </summary>
        public async Task<IEnumerable<Dom.Deposito>> GetActivosAsync()
        {
            var efDepositos = await _context.Depositos
                .Where(d => d.Activo)
                .Include(d => d.Estantes.Where(e => e.Activo))
                .AsNoTracking()
                .ToListAsync();

            return _depositoMapper.ToDomain(efDepositos);
        }
    }
}