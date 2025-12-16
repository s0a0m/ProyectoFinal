using Microsoft.EntityFrameworkCore;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;
using src.Repositories.Interfaces;
using src.Models.Mappers;
using src.Contracts;
using src.Models.CodeFirst;
namespace src.Repositories.Implementations;

public class ProductoProveedorRepository : IProductoProveedorRepository
{
    private readonly EF.AppDbContext _context;

    public ProductoProveedorRepository(EF.AppDbContext context)
    {
        _context = context;
    }

    private IQueryable<EF.ProductoProveedor> GetQueryFull()
    {
        return _context.ProductosProveedores
            .Include(pp => pp.Producto)
                .ThenInclude(p => p.CodigosBarrasExternos)
            .Include(pp => pp.Proveedor)
                .ThenInclude(p => p.IdCondicionPagoHabitualNavigation);
    }

    public async Task<IEnumerable<ProductoProveedorDto>> GetAllAsync()
    {
        var efEntities = await GetQueryFull().Where(x => x.Activo).ToListAsync();
        var resultado = new List<ProductoProveedorDto>();

        foreach (var efItem in efEntities)
        {
            // --- CAMBIO CRÍTICO: MAPEO MANUAL ---
            // NO usamos DominioMapper.Map(efItem) porque intenta mapear Domicilios/Provincias
            // que no trajimos de la BD, causando NullReferenceException.

            var domItem = new Dom.ProductoProveedor
            {
                Precio = efItem.Precio,
                StockAsignado = efItem.StockAsignado,
                Activo = efItem.Activo,
                // Construimos el Producto manualmente (Solo lo necesario)
                Producto = efItem.Producto == null ? null : new Dom.Producto
                {
                    IdProducto = efItem.Producto.IdProducto,
                    Nombre = efItem.Producto.Nombre
                },

                // Construimos el Proveedor manualmente (Solo lo necesario)
                Proveedor = efItem.Proveedor == null ? null : new Dom.Proveedor
                {
                    IdProveedor = efItem.Proveedor.IdProveedor,
                    RazonSocial = efItem.Proveedor.RazonSocial
                    // NO asignamos IdDomicilioNavigation para que el mapper no moleste
                }
            };

            // Obtenemos lista de códigos
            var listaCodigos = efItem.Producto.CodigosBarrasExternos
                .Where(c => c.IdProveedor == efItem.IdProveedor)
                .Select(c => c.CodigoBarraProveedor)
                .ToList();

            resultado.Add(new ProductoProveedorDto
            {
                ProductoProveedor = domItem,
                CodigosBarrasExternos = listaCodigos
            });
        }

        return resultado;
    }
    public async Task<ProductoProveedorDto?> GetByIdAsync(int idProducto, int idProveedor)
    {
        var efEntity = await GetQueryFull()
            .FirstOrDefaultAsync(x => x.IdProducto == idProducto && x.IdProveedor == idProveedor);

        if (efEntity == null) return null;

        // --- CAMBIO CRÍTICO: MAPEO MANUAL TAMBIÉN AQUÍ ---
        var dom = new Dom.ProductoProveedor
        {
            Precio = efEntity.Precio,
            StockAsignado = efEntity.StockAsignado,
            Activo = efEntity.Activo,
            Producto = efEntity.Producto == null ? null : new Dom.Producto
            {
                IdProducto = efEntity.Producto.IdProducto,
                Nombre = efEntity.Producto.Nombre
            },

            Proveedor = efEntity.Proveedor == null ? null : new Dom.Proveedor
            {
                IdProveedor = efEntity.Proveedor.IdProveedor,
                RazonSocial = efEntity.Proveedor.RazonSocial
            }
        };

        var listaCodigos = efEntity.Producto.CodigosBarrasExternos
            .Where(c => c.IdProveedor == efEntity.IdProveedor)
            .Select(c => c.CodigoBarraProveedor)
            .ToList();

        return new ProductoProveedorDto
        {
            ProductoProveedor = dom,
            CodigosBarrasExternos = listaCodigos
        };
    }
    public async Task<IEnumerable<Dom.ProductoProveedor>> GetByProveedorIdAsync(int idProveedor)
    {
        var efEntities = await GetQueryFull()
            .Where(x => x.IdProveedor == idProveedor)
            .ToListAsync();

        return DominioMapper.Map(efEntities);
    }



    public async Task AddAsync(Dom.ProductoProveedor entity, List<string> codigosExternos)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var efEntity = new EF.ProductoProveedor
            {
                IdProducto = (short)entity.Producto.IdProducto,
                IdProveedor = (short)entity.Proveedor.IdProveedor,
                Precio = entity.Precio,
                StockAsignado = entity.StockAsignado,
                Activo = true
            };

            await _context.ProductosProveedores.AddAsync(efEntity);
            await _context.SaveChangesAsync();

            // --- CAMBIO: Iteramos la lista para guardar múltiples códigos ---
            if (codigosExternos != null && codigosExternos.Any())
            {
                foreach (var codigoStr in codigosExternos)
                {
                    if (string.IsNullOrWhiteSpace(codigoStr)) continue;

                    var nuevoCodigo = new ProductoCodigoExterno
                    {
                        IdProducto = efEntity.IdProducto,
                        IdProveedor = efEntity.IdProveedor,
                        CodigoBarraProveedor = codigoStr.Trim()
                    };
                    await _context.ProductoCodigosExternos.AddAsync(nuevoCodigo);
                }
                await _context.SaveChangesAsync();
            }

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }








    public async Task UpdateAsync(Dom.ProductoProveedor entity, CancellationToken cancellationToken = default)
    {
        var idProducto = entity.Producto?.IdProducto ?? 0;
        var idProveedor = entity.Proveedor?.IdProveedor ?? 0;

        var existingEfEntity = await _context.ProductosProveedores
            .FirstOrDefaultAsync(x => x.IdProducto == idProducto && x.IdProveedor == idProveedor, cancellationToken);

        if (existingEfEntity == null)
        {
            throw new KeyNotFoundException("Relación Producto-Proveedor no encontrada para actualización.");
        }

        existingEfEntity.Precio = entity.Precio;
        existingEfEntity.StockAsignado = entity.StockAsignado;

        _context.ProductosProveedores.Update(existingEfEntity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DesactivarPorProductoAsync(int idProducto)
    {
        var relaciones = await _context.ProductosProveedores
            .Where(pp => pp.IdProducto == idProducto && pp.Activo)
            .ToListAsync();

        if (relaciones.Any())
        {
            foreach (var rel in relaciones)
            {
                rel.Activo = false;
            }
            await _context.SaveChangesAsync();
        }
    }

    public async Task DesactivarPorProveedorAsync(int idProveedor)
    {
        var relaciones = await _context.ProductosProveedores
            .Where(pp => pp.IdProveedor == idProveedor && pp.Activo)
            .ToListAsync();

        if (relaciones.Any())
        {
            foreach (var rel in relaciones)
            {
                rel.Activo = false;
            }
            await _context.SaveChangesAsync();
        }
    }

    public async Task ReactivarPorProductoAsync(int idProducto)
    {
        var relaciones = await _context.ProductosProveedores
            .Include(pp => pp.Proveedor)
            .Where(pp => pp.IdProducto == idProducto && !pp.Activo)
            .ToListAsync();

        if (relaciones.Any())
        {
            foreach (var rel in relaciones)
            {

                if (rel.Proveedor != null && rel.Proveedor.Activo)
                {
                    rel.Activo = true;
                }
            }
            await _context.SaveChangesAsync();
        }
    }

    public async Task ReactivarPorProveedorAsync(int idProveedor)
    {

        var relaciones = await _context.ProductosProveedores
            .Include(pp => pp.Producto)
            .Where(pp => pp.IdProveedor == idProveedor && !pp.Activo)
            .ToListAsync();

        if (relaciones.Any())
        {
            foreach (var rel in relaciones)
            {
                if (rel.Producto != null && rel.Producto.Activo)
                {
                    rel.Activo = true;
                }
            }
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int idProducto, int idProveedor)
    {
        return await _context.ProductosProveedores
            .AnyAsync(x => x.IdProducto == idProducto && x.IdProveedor == idProveedor);
    }
}
