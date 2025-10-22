using Microsoft.EntityFrameworkCore;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;
using src.Repositories.Interfaces;
using src.Migrations;
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
        return  _context.Proveedores
        .Include(p => p.IdCondicionPagoHabitualNavigation)
        .Include(p => p.IdDomicilioNavigation)
            .ThenInclude(p => p.IdProvinciaNavigation);
    }

    public async Task<IEnumerable<Dom.Proveedor>> GetAllProveedorAsync()
    {
        IEnumerable<EF.Proveedor> provEF = await GetQueryProveedor().ToListAsync();
        IEnumerable<Dom.Proveedor> proveedores = DominioMapper.Map(provEF);
        return  proveedores;
    }

    public async Task<Dom.Proveedor?> GetProveedorById(int idProv)
    {
        EF.Proveedor? proveedorEF = await GetQueryProveedor().Where(p => p.IdProveedor == idProv).FirstOrDefaultAsync();
        if (proveedorEF is null) return null;
        Dom.Proveedor proveedorDOM = DominioMapper.Map(proveedorEF);
        return proveedorDOM;
    }


    public async Task AddAsync(Dom.Proveedor entity)
    {
        EF.Proveedor proveedorEF = DominioMapper.Map(entity);
        await _context.Proveedores.AddAsync(proveedorEF);
        await _context.SaveChangesAsync();
    }

   
    public async Task UpdateAsync(Dom.Proveedor entity)
    {
        EF.Proveedor proveedorEF = DominioMapper.Map(entity);
        _context.Proveedores.Update(proveedorEF);
        await _context.SaveChangesAsync();
    }


    public async Task<bool> DeleteAsync(int id)
    {

        var proveedorEF = await _context.Proveedores
            .FirstOrDefaultAsync(p => p.IdProveedor== id);
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