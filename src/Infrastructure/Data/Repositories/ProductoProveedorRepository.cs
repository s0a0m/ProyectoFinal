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
            

            var domItem = new Dom.ProductoProveedor
            {
                Precio = efItem.Precio,
                StockAsignado = efItem.StockAsignado,
                Activo = efItem.Activo,
                
                Producto = efItem.Producto == null ? null : new Dom.Producto
                {
                    IdProducto = efItem.Producto.IdProducto,
                    Nombre = efItem.Producto.Nombre
                },

               
                Proveedor = efItem.Proveedor == null ? null : new Dom.Proveedor
                {
                    IdProveedor = efItem.Proveedor.IdProveedor,
                    RazonSocial = efItem.Proveedor.RazonSocial
              
                }
            };

            
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








    public async Task UpdateAsync(Dom.ProductoProveedor entity,List<string> nuevosCodigosExternos, CancellationToken cancellationToken = default)
    {
        var idProducto = entity.Producto?.IdProducto ?? 0;
        var idProveedor = entity.Proveedor?.IdProveedor ?? 0;

        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var existingEfEntity = await _context.ProductosProveedores
                .FirstOrDefaultAsync(x => x.IdProducto == idProducto && x.IdProveedor == idProveedor, cancellationToken);

            if (existingEfEntity == null)
                throw new KeyNotFoundException("Relación Producto-Proveedor no encontrada para actualización.");

            existingEfEntity.Precio = entity.Precio;
            existingEfEntity.StockAsignado = entity.StockAsignado;

            _context.ProductosProveedores.Update(existingEfEntity);
            await _context.SaveChangesAsync(cancellationToken);

            // Insertar solo los nuevos códigos (no toca los existentes)
            if (nuevosCodigosExternos != null && nuevosCodigosExternos.Any())
            {
                foreach (var codigoStr in nuevosCodigosExternos)
                {
                    if (string.IsNullOrWhiteSpace(codigoStr)) continue;

                    var nuevoCodigo = new ProductoCodigoExterno
                    {
                        IdProducto = existingEfEntity.IdProducto,
                        IdProveedor = existingEfEntity.IdProveedor,
                        CodigoBarraProveedor = codigoStr.Trim()
                    };
                    await _context.ProductoCodigosExternos.AddAsync(nuevoCodigo, cancellationToken);
                }
                await _context.SaveChangesAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task UpdateAsync2(Dom.ProductoProveedor entity)
    {
        // Buscar por clave compuesta (Producto + Proveedor)
        if (entity.Producto == null || entity.Proveedor == null)
        {
            throw new ArgumentException("La entidad ProductoProveedor debe tener Producto y Proveedor asignados para actualizar.");
        }
        var idProducto = entity.Producto.IdProducto;
        var idProveedor = entity.Proveedor.IdProveedor;
        var efEntity = await _context.ProductosProveedores
            .FirstOrDefaultAsync(pp => pp.IdProducto == idProducto 
                                    && pp.IdProveedor == idProveedor);

        if (efEntity != null)
        {
            // Actualizar campos
            efEntity.Precio = entity.Precio;
            efEntity.StockAsignado = entity.StockAsignado;
            efEntity.Activo = entity.Activo;

            // Guardar
            await _context.SaveChangesAsync();
        }
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

    public async Task AddAsync(Dom.ProductoProveedor entity)
    {
        // Mapeo manual directo a EF (igual que en tu método original)
        var efEntity = new EF.ProductoProveedor
        {
            IdProducto = (short)entity.Producto.IdProducto,
            IdProveedor = (short)entity.Proveedor.IdProveedor,
            Precio = entity.Precio,
            StockAsignado = entity.StockAsignado,
            Activo = true // O entity.Activo si quieres respetar lo que viene
        };

        await _context.ProductosProveedores.AddAsync(efEntity);
        await _context.SaveChangesAsync();
    }

    public async Task<Dom.ProductoProveedor?> GetByProductoAndProveedorAsync(short idProducto, short idProveedor)
    {
        var efEntity = await _context.ProductosProveedores
            .Include(pp => pp.Producto)
            .Include(pp => pp.Proveedor)
            .FirstOrDefaultAsync(x => x.IdProducto == idProducto && x.IdProveedor == idProveedor);

        if (efEntity == null) return null;

        // MAPEO MANUAL SEGURO (Evita que el DominioMapper devuelva nulls)
        return new Dom.ProductoProveedor
        {
            Producto = new Dom.Producto { IdProducto = efEntity.IdProducto },
            Proveedor = new Dom.Proveedor { IdProveedor = efEntity.IdProveedor },
            Precio = efEntity.Precio,
            StockAsignado = efEntity.StockAsignado,
            Activo = efEntity.Activo
        };
    }


}
