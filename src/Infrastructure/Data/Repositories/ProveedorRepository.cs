using Microsoft.EntityFrameworkCore;
using src.Models.Mappers;
using src.Repositories.Interfaces;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;

namespace src.Repositories.Implementations;

public class ProveedorRepository : IProveedorRepository
{
    private readonly EF.AppDbContext _context;
    private readonly ProveedorMapper _proveedorMapper;
    private readonly DomicilioMapper _domicilioMapper;

    public ProveedorRepository(
        EF.AppDbContext context,
        ProveedorMapper proveedorMapper,
        DomicilioMapper domicilioMapper
    )
    {
        _context = context;
        _proveedorMapper = proveedorMapper;
        _domicilioMapper = domicilioMapper;
    }

    private IQueryable<EF.Proveedor> GetQueryProveedor()
    {
        return _context
            .Proveedores.Include(p => p.IdCondicionPagoHabitualNavigation)
            .Include(p => p.IdDomicilioNavigation)
                .ThenInclude(p => p.IdProvinciaNavigation);
    }

    public async Task<IEnumerable<Dom.Proveedor>> GetAllProveedorAsync()
    {
        IEnumerable<EF.Proveedor> provEF = await GetQueryProveedor().ToListAsync();
        IEnumerable<Dom.Proveedor> proveedores = _proveedorMapper.ToDomain(provEF);
        return proveedores;
    }

    public async Task<Dom.Proveedor?> GetProveedorById(int idProv)
    {
        EF.Proveedor? proveedorEF = await GetQueryProveedor()
            .Where(p => p.IdProveedor == idProv)
            .FirstOrDefaultAsync();
        return proveedorEF is null ? null : _proveedorMapper.ToDomain(proveedorEF);
    }

    public async Task<Dom.Proveedor?> GetByCuitAsync(string cuit)
    {
        if (string.IsNullOrWhiteSpace(cuit))
            return null;

        EF.Proveedor? proveedorEF = await GetQueryProveedor()
            .Where(p => p.Cuit.Trim() == cuit.Trim())
            .FirstOrDefaultAsync();
        
        return proveedorEF is null ? null : _proveedorMapper.ToDomain(proveedorEF);
    }

    public async Task AddAsync(Dom.Proveedor entity)
    {
        EF.Proveedor proveedorEF = _proveedorMapper.ToEntity(entity);
        proveedorEF.IdProveedor = 0;
        proveedorEF.IdDomicilio = 0;
        proveedorEF.IdCondicionPagoHabitual = 0;
        await _context.Proveedores.AddAsync(proveedorEF);
        await _context.SaveChangesAsync();
        entity.IdProveedor = proveedorEF.IdProveedor;
        entity.Direccion.IdDomicilio = proveedorEF.IdDomicilio;
        if (entity.Direccion != null && proveedorEF.IdDomicilioNavigation != null)
        {
            entity.Direccion.IdDomicilio = proveedorEF.IdDomicilioNavigation.IdDomicilio;
        }

        if (entity.Condicion != null && proveedorEF.IdCondicionPagoHabitualNavigation != null)
        {
            entity.Condicion.IdCondicionPago = proveedorEF
                .IdCondicionPagoHabitualNavigation
                .IdCondicionPago;
        }
    }

    public async Task UpdateAsync(Dom.Proveedor entity)
    {
        var existingEntity = await _context
            .Proveedores.Include(p => p.IdDomicilioNavigation)
            .Include(p => p.IdCondicionPagoHabitualNavigation)
            .FirstOrDefaultAsync(p => p.IdProveedor == entity.IdProveedor);

        if (existingEntity == null)
        {
            throw new InvalidOperationException(
                $"Proveedor con ID {entity.IdProveedor} no encontrado."
            );
        }

        // Map the incoming domain object to a temporary EF object
        EF.Proveedor proveedorTemporalEF = _proveedorMapper.ToEntity(entity);

        // --- Update Proveedor Scalar Values ---
        _context.Entry(existingEntity).CurrentValues.SetValues(proveedorTemporalEF);

        // --- Explicit Handling for Domicilio ---
        if (existingEntity.IdDomicilioNavigation != null && entity.Direccion != null)
        {
            // Map the updated Domain Direccion to a temporary EF Domicilio
            // Note: Make sure your mapper handles the Provincia FK correctly here!
            // It should map Dom.Direccion.Prov.IdProvincia -> EF.Domicilio.IdProvincia
            EF.Domicilio domicilioTemporalEF = _domicilioMapper.ToEntity(entity.Direccion);

            // Apply changes to the TRACKED DomicilioNavigation
            _context
                .Entry(existingEntity.IdDomicilioNavigation)
                .CurrentValues.SetValues(domicilioTemporalEF);

            // Explicitly ensure the Provincia Foreign Key is set on the tracked Domicilio
            // This relies on the controller having correctly assigned entity.Direccion.Prov
            if (entity.Direccion.Prov != null)
            {
                existingEntity.IdDomicilioNavigation.IdProvincia = entity
                    .Direccion
                    .Prov
                    .IdProvincia;
            }
            else
            {
                // Handle cases where Provincia might become null if allowed by DB
                // existingEntity.IdDomicilioNavigation.IdProvincia = 0; // Or appropriate default/null FK
            }
        }
        else if (entity.Direccion != null)
        {
            // Handle case: Existing provider had no address, but now one is added.
            // This is less likely if Direccion is required, but good practice.
            EF.Domicilio nuevoDomicilioEF = _domicilioMapper.ToEntity(entity.Direccion);
            nuevoDomicilioEF.IdDomicilio = 0; // Ensure EF knows it's new
            _context.Add(nuevoDomicilioEF);
            existingEntity.IdDomicilioNavigation = nuevoDomicilioEF; // Associate
        }
        // else if (entity.Direccion == null && existingEntity.IdDomicilioNavigation != null)
        // {
        //     // Handle case: Address is removed (if allowed)
        //     _context.Remove(existingEntity.IdDomicilioNavigation);
        //     existingEntity.IdDomicilioNavigation = null;
        // }

        // Set the Proveedor's FK explicitly AFTER potentially adding a new Domicilio.
        // If a new Domicilio was added, EF Core handles assigning its ID to this FK.
        // If only values were updated, this ensures the FK remains correct.
        // **Important**: Rely on EF Core to manage the FK when adding/associating navigations.
        // This explicit setting might be redundant or even problematic if adding new related entities.
        // Let's comment it out for now and rely on navigation property association.
        // existingEntity.IdDomicilio = proveedorTemporalEF.IdDomicilio;

        // --- Explicit Handling for CondicionDePago (Keep previous logic) ---
        bool conditionChanged = false;
        EF.CondicionDePago? newConditionEF = null;

        if (entity.Condicion != null)
        {
            newConditionEF = DominioMapper.Map(entity.Condicion);
            if (
                existingEntity.IdCondicionPagoHabitualNavigation == null
                || existingEntity.IdCondicionPagoHabitualNavigation.GetType()
                    != newConditionEF.GetType()
                || existingEntity.IdCondicionPagoHabitual != newConditionEF.IdCondicionPago
            )
            {
                conditionChanged = true;
                if (existingEntity.IdCondicionPagoHabitualNavigation != null)
                {
                    _context.Remove(existingEntity.IdCondicionPagoHabitualNavigation);
                }
                newConditionEF.IdCondicionPago = 0;
                _context.Add(newConditionEF);
                existingEntity.IdCondicionPagoHabitualNavigation = newConditionEF;
            }
            else
            {
                _context
                    .Entry(existingEntity.IdCondicionPagoHabitualNavigation)
                    .CurrentValues.SetValues(newConditionEF);
            }
        }
        else if (existingEntity.IdCondicionPagoHabitualNavigation != null)
        {
            _context.Remove(existingEntity.IdCondicionPagoHabitualNavigation);
            existingEntity.IdCondicionPagoHabitualNavigation = null;
            existingEntity.IdCondicionPagoHabitual = 0; // Or null if FK is nullable
            conditionChanged = true;
        }

        // --- Save Changes ---
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var proveedorEF = await _context.Proveedores.FirstOrDefaultAsync(p => p.IdProveedor == id);
        if (proveedorEF == null)
            return false;
        proveedorEF.Activo = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReactivateAsync(int id)
    {
        var proveedorEF = await _context.Proveedores.FirstOrDefaultAsync(p => p.IdProveedor == id);

        if (proveedorEF == null)
            return false;

        proveedorEF.Activo = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task ActualizarSaldoActualAsync(int idProveedor, decimal nuevoSaldo)
    {
        var proveedor = await _context.Proveedores.FirstOrDefaultAsync(p =>
            p.IdProveedor == idProveedor
        );

        if (proveedor == null)
            throw new Exception("Proveedor no encontrado");

        proveedor.SaldoActual = nuevoSaldo;

        await _context.SaveChangesAsync();
    }
}
