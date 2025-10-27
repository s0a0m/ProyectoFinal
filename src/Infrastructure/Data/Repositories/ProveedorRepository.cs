using Microsoft.EntityFrameworkCore;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;
using src.Repositories.Interfaces;
using src.Models.Mappers;

namespace src.Repositories.Implementations;

public class ProveedorRepository : IProveedorRepository
{
    private readonly EF.AppDbContext _context;

    public ProveedorRepository(EF.AppDbContext context)
    {
        _context = context;
    }

    private IQueryable<EF.Proveedor> GetQueryProveedor()
    {
        return _context.Proveedores
        .Include(p => p.IdCondicionPagoHabitualNavigation)
        .Include(p => p.IdDomicilioNavigation)
            .ThenInclude(p => p.IdProvinciaNavigation);
    }

    public async Task<IEnumerable<Dom.Proveedor>> GetAllProveedorAsync()
    {
        IEnumerable<EF.Proveedor> provEF = await GetQueryProveedor().ToListAsync();
        IEnumerable<Dom.Proveedor> proveedores = DominioMapper.Map(provEF);
        return proveedores;
    }

    public async Task<Dom.Proveedor?> GetProveedorById(int idProv)
    {
        EF.Proveedor? proveedorEF = await GetQueryProveedor().Where(p => p.IdProveedor == idProv).FirstOrDefaultAsync();
        return proveedorEF is null ? null : DominioMapper.Map(proveedorEF);
    }


    public async Task AddAsync(Dom.Proveedor entity)
    {
        EF.Proveedor proveedorEF = DominioMapper.Map(entity);
        proveedorEF.IdProveedor = 0;
        proveedorEF.IdDomicilio = 0;
        proveedorEF.IdCondicionPagoHabitual = 0;
        await _context.Proveedores.AddAsync(proveedorEF);
        await _context.SaveChangesAsync();
        entity.IdProveedor = proveedorEF.IdProveedor;
        entity.Direccion.IdDomicilio = proveedorEF.IdDomicilio;
        if (entity.Condicion != null)
        {
            entity.Condicion.IdCondicionPago = proveedorEF.IdCondicionPagoHabitual;
        }
    }




    public async Task UpdateAsync(Dom.Proveedor entity)
    {
        var existingEntity = await _context.Proveedores
                                        .Include(p => p.IdDomicilioNavigation)
                                        .Include(p => p.IdCondicionPagoHabitualNavigation)
                                        .FirstOrDefaultAsync(p => p.IdProveedor == entity.IdProveedor);

        if (existingEntity == null)
        {
            throw new InvalidOperationException($"Proveedor con ID {entity.IdProveedor} no encontrado.");
        }

        // Map the incoming domain object to a temporary EF object
        EF.Proveedor proveedorTemporalEF = DominioMapper.Map(entity);

        // --- Update Proveedor Scalar Values ---
        _context.Entry(existingEntity).CurrentValues.SetValues(proveedorTemporalEF);

        // --- Explicit Handling for Domicilio ---
        if (existingEntity.IdDomicilioNavigation != null && entity.Direccion != null)
        {
            // Map the updated Domain Direccion to a temporary EF Domicilio
            // Note: Make sure your mapper handles the Provincia FK correctly here!
            // It should map Dom.Direccion.Prov.IdProvincia -> EF.Domicilio.IdProvincia
            EF.Domicilio domicilioTemporalEF = DominioMapper.Map(entity.Direccion);

            // Apply changes to the TRACKED DomicilioNavigation
            _context.Entry(existingEntity.IdDomicilioNavigation).CurrentValues.SetValues(domicilioTemporalEF);

            // Explicitly ensure the Provincia Foreign Key is set on the tracked Domicilio
            // This relies on the controller having correctly assigned entity.Direccion.Prov
            if (entity.Direccion.Prov != null)
            {
                existingEntity.IdDomicilioNavigation.IdProvincia = entity.Direccion.Prov.IdProvincia;
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
            EF.Domicilio nuevoDomicilioEF = DominioMapper.Map(entity.Direccion);
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
            if (existingEntity.IdCondicionPagoHabitualNavigation == null ||
                existingEntity.IdCondicionPagoHabitualNavigation.GetType() != newConditionEF.GetType() ||
                existingEntity.IdCondicionPagoHabitual != newConditionEF.IdCondicionPago)
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
                _context.Entry(existingEntity.IdCondicionPagoHabitualNavigation).CurrentValues.SetValues(newConditionEF);
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

        var proveedorEF = await _context.Proveedores
            .FirstOrDefaultAsync(p => p.IdProveedor == id);
        if (proveedorEF == null)
            return false;
        proveedorEF.Activo = false;
        await _context.SaveChangesAsync();
        return true;
    }
}


/* METODOS QUE SUGIERE CHATGPT PARA UN FUTURO 

 public async Task<bool> ToggleActivoAsync(int id)
        {
            var proveedorEF = await _context.proveedores.FindAsync(id);
            if (proveedorEF == null) return false;

            proveedorEF.activo = !proveedorEF.activo;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetSaldoTotalAsync()
        {
            return await _context.proveedores
                .Where(p => p.activo)
                .SumAsync(p => p.saldo);
        }

        public async Task<bool> ExistsByCuitAsync(string cuit, int? excludeId = null)
        {
            var query = _context.proveedores.Where(p => p.cuit == cuit);
            
            if (excludeId.HasValue)
                query = query.Where(p => p.id_proveedor != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<List<Proveedor>> GetProveedoresConSaldoPositivoAsync()
        {
            var proveedoresEF = await _context.proveedores
                .Include(p => p.id_condicion_pago_habitualNavigation)
                .Include(p => p.id_domicilioNavigation)
                    .ThenInclude(d => d.id_provinciaNavigation)
                .Where(p => p.activo && p.saldo > 0)
                .ToListAsync();

            var proveedores = new List<Proveedor>();
            foreach (var proveedorEF in proveedoresEF)
            {
                var proveedor = _mapper.Map<Proveedor>(proveedorEF);
                proveedor.condicion = await MapCondicionPagoAsync(proveedorEF.id_condicion_pago_habitual);
                proveedores.Add(proveedor);
            }

            return proveedores;
        }

        public async Task<List<Proveedor>> GetProveedoresConDireccionCompletaAsync()
        {
            var proveedoresEF = await _context.proveedores
                .Include(p => p.id_condicion_pago_habitualNavigation)
                .Include(p => p.id_domicilioNavigation)
                    .ThenInclude(d => d.id_provinciaNavigation)
                .Where(p => p.activo && p.id_domicilioNavigation != null)
                .ToListAsync();

            var proveedores = new List<Proveedor>();
            foreach (var proveedorEF in proveedoresEF)
            {
                var proveedor = _mapper.Map<Proveedor>(proveedorEF);
                proveedor.condicion = await MapCondicionPagoAsync(proveedorEF.id_condicion_pago_habitual);
                proveedores.Add(proveedor);
            }

            return proveedores;
        }*/