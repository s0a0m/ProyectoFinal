using Microsoft.EntityFrameworkCore;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;
using src.Repositories.Interfaces;
using src.Models.Mappers;

namespace src.Repositories.Implementations;

public class ProductoProveedorRepository : IProductoProveedorRepository
{
    private readonly EF.AppDbContext _context;

    public ProductoProveedorRepository(EF.AppDbContext context)
    {
        _context = context;
    }

    // Método privado para reutilizar la query con includes
    private IQueryable<EF.ProductoProveedor> GetQueryFull()
    {
        return _context.ProductosProveedores
            .Include(pp => pp.Producto)
            .Include(pp => pp.Proveedor)
            .ThenInclude(p => p.IdCondicionPagoHabitualNavigation) // Traemos datos anidados si son necesarios
            .Include(pp => pp.Proveedor)
            .ThenInclude(p => p.IdDomicilioNavigation);
    }

    public async Task<IEnumerable<Dom.ProductoProveedor>> GetAllAsync()
    {
        var efEntities = await GetQueryFull().ToListAsync();
        return DominioMapper.Map(efEntities);
    }

    public async Task<Dom.ProductoProveedor?> GetByIdAsync(int idProducto, int idProveedor)
    {
        var efEntity = await GetQueryFull()
            .FirstOrDefaultAsync(x => x.IdProducto == idProducto && x.IdProveedor == idProveedor);

        return efEntity == null ? null : DominioMapper.Map(efEntity);
    }

    public async Task<IEnumerable<Dom.ProductoProveedor>> GetByProveedorIdAsync(int idProveedor)
    {
        var efEntities = await GetQueryFull()
            .Where(x => x.IdProveedor == idProveedor)
            .ToListAsync();

        return DominioMapper.Map(efEntities);
    }

    public async Task AddAsync(Dom.ProductoProveedor entity)
    {
        // Mapeamos del Dominio a CodeFirst
        var efEntity = DominioMapper.Map(entity);

        // Validación extra: Asegurarnos que no intentamos insertar duplicados
        if (await ExistsAsync(efEntity.IdProducto, efEntity.IdProveedor))
        {
            throw new InvalidOperationException($"La relación entre Producto {efEntity.IdProducto} y Proveedor {efEntity.IdProveedor} ya existe.");
        }

        await _context.ProductosProveedores.AddAsync(efEntity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Dom.ProductoProveedor entity)
    {
        // 1. Obtenemos los IDs del objeto de dominio
        // Asumimos que entity.Producto y entity.Proveedor no son nulos y tienen sus IDs
        var idProducto = entity.Producto?.IdProducto ?? 0;
        var idProveedor = entity.Proveedor?.IdProveedor ?? 0;

        // 2. Buscamos la entidad existente en la DB (sin tracking para optimizar si fuéramos a reemplazar, 
        // pero aquí necesitamos tracking para actualizar campos específicos)
        var existingEfEntity = await _context.ProductosProveedores
            .FirstOrDefaultAsync(x => x.IdProducto == idProducto && x.IdProveedor == idProveedor);

        if (existingEfEntity == null)
        {
            throw new KeyNotFoundException($"No se encontró la relación Producto-Proveedor para actualizar.");
        }

        // 3. Actualizamos SOLO los campos permitidos (Precio y Stock)
        // No tocamos los IDs ni las relaciones
        existingEfEntity.Precio = entity.Precio;
        existingEfEntity.StockAsignado = entity.StockAsignado;

        // 4. Guardamos
        _context.ProductosProveedores.Update(existingEfEntity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int idProducto, int idProveedor)
    {
        var entity = await _context.ProductosProveedores
            .FirstOrDefaultAsync(x => x.IdProducto == idProducto && x.IdProveedor == idProveedor);

        if (entity != null)
        {
            _context.ProductosProveedores.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    // VALIDACIÓN / UTILIDAD
    public async Task<bool> ExistsAsync(int idProducto, int idProveedor)
    {
        return await _context.ProductosProveedores
            .AnyAsync(x => x.IdProducto == idProducto && x.IdProveedor == idProveedor);
    }
}