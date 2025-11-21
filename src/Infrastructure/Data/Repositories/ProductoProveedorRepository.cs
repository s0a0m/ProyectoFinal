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

        // Query con Includes
        private IQueryable<EF.ProductoProveedor> GetQueryFull()
        {
            return _context.ProductosProveedores
                .Include(pp => pp.Producto)
                    .ThenInclude(p => p.CodigosBarrasExternos)
                .Include(pp => pp.Proveedor)
                    .ThenInclude(p => p.IdCondicionPagoHabitualNavigation)
            .Include(pp => pp.Proveedor)
                .ThenInclude(p => p.IdDomicilioNavigation)
                .ThenInclude(d => d.IdProvinciaNavigation); 
        }   

    
        /*public async Task<IEnumerable<ProductoProveedorDto>> GetAllAsync()
        {
            var efEntities = await GetQueryFull().ToListAsync();

            var dominio = DominioMapper.Map(efEntities);

            var resultado = new List<ProductoProveedorDto>();

            foreach (var domItem in dominio)
            {
                var efItem = efEntities.First(x =>
                    x.IdProducto == domItem.Producto.IdProducto &&
                    x.IdProveedor == domItem.Proveedor.IdProveedor
                );

                // Buscar código externo asociado a este proveedor
                var codigoExterno = efItem.Producto.CodigosBarrasExternos
                    .FirstOrDefault(c => c.IdProveedor == efItem.IdProveedor)?
                    .CodigoBarraProveedor ?? "";

                resultado.Add(new ProductoProveedorDto
                {
                    ProductoProveedor = domItem,
                    CodigoBarraExterno = codigoExterno
                });
            }

            return resultado;
        }*/
        public async Task<IEnumerable<ProductoProveedorDto>> GetAllAsync()
        {
        // Traemos los datos de EF
        var efEntities = await GetQueryFull().ToListAsync();
        var resultado = new List<ProductoProveedorDto>();

        // Iteramos DIRECTAMENTE sobre la entidad de EF.
        // Así tenemos acceso al objeto original y podemos mapear al vuelo.
        foreach (var efItem in efEntities)
        {
            // Mapeamos este item individual al dominio
            var domItem = DominioMapper.Map(efItem);

            // PARCHE DE SEGURIDAD:
            // Si por alguna razón el Mapper devuelve el Producto nulo, lo forzamos manualmente
            // para evitar que la Vista o el Servicio exploten después.
            if (domItem.Producto == null && efItem.Producto != null)
            {
                domItem.Producto = new Dom.Producto 
                { 
                    IdProducto = efItem.IdProducto, 
                    Nombre = efItem.Producto.Nombre 
                };
            }
            if (domItem.Proveedor == null && efItem.Proveedor != null)
            {
                domItem.Proveedor = new Dom.Proveedor
                {
                    IdProveedor = efItem.IdProveedor,
                    RazonSocial = efItem.Proveedor.RazonSocial
                };
            }

            // Extraemos el código de barras usando el efItem que ya tenemos en la mano
            // (No hace falta usar .First ni buscar nada)
            var codigoExterno = efItem.Producto.CodigosBarrasExternos
                .FirstOrDefault(c => c.IdProveedor == efItem.IdProveedor)?
                .CodigoBarraProveedor ?? "";

            resultado.Add(new ProductoProveedorDto
            {
                ProductoProveedor = domItem,
                CodigoBarraExterno = codigoExterno
            });
        }

        return resultado;
        }

        public async Task<ProductoProveedorDto?> GetByIdAsync(int idProducto, int idProveedor)
        {
            var efEntity = await GetQueryFull()
                .FirstOrDefaultAsync(x => x.IdProducto == idProducto && x.IdProveedor == idProveedor);

            if (efEntity == null)
                return null;

            var dom = DominioMapper.Map(efEntity);

            var codigoExterno = efEntity.Producto.CodigosBarrasExternos
                .FirstOrDefault(c => c.IdProveedor == idProveedor)?
                .CodigoBarraProveedor ?? "";

            return new ProductoProveedorDto
            {
                ProductoProveedor = dom,
                CodigoBarraExterno = codigoExterno
            };
        }

        public async Task<IEnumerable<Dom.ProductoProveedor>> GetByProveedorIdAsync(int idProveedor)
        {
            var efEntities = await GetQueryFull()
                .Where(x => x.IdProveedor == idProveedor)
                .ToListAsync();

            return DominioMapper.Map(efEntities);
        }

    public async Task AddAsync(Dom.ProductoProveedor entity, string codigoExterno)
    {
    // 1. CORRECCIÓN CRÍTICA: Mapeo Manual
    // No usamos el Mapper aquí para evitar que EF intente insertar
    // el Producto y el Proveedor nuevamente como si fueran nuevos.
    var efEntity = new EF.ProductoProveedor
    {
        // Asignamos SOLO las claves foráneas (IDs)
        IdProducto = (short)entity.Producto.IdProducto,
        IdProveedor = (short)entity.Proveedor.IdProveedor,
        
        // Datos propios de la tabla intermedia
        Precio = entity.Precio,
        StockAsignado = entity.StockAsignado
        
        // IMPORTANTE: NO asignamos efEntity.Producto ni efEntity.Proveedor
        // Dejarlos en null le dice a EF: "Estos ya existen, solo guarda la relación".
    };
    
    await _context.ProductosProveedores.AddAsync(efEntity);
    await _context.SaveChangesAsync();

    // 2. Guardar Código Externo (Solo si el usuario escribió algo)
    if (!string.IsNullOrWhiteSpace(codigoExterno))
    {
        var codigo = new ProductoCodigoExterno 
        {
            IdProducto = efEntity.IdProducto,
            IdProveedor = efEntity.IdProveedor,
            CodigoBarraProveedor = codigoExterno.Trim()
        };

        await _context.ProductoCodigosExternos.AddAsync(codigo);
        await _context.SaveChangesAsync();
        }
    }


        public async Task UpdateAsync(Dom.ProductoProveedor entity)
        {
            var idProducto = entity.Producto?.IdProducto ?? 0;
            var idProveedor = entity.Proveedor?.IdProveedor ?? 0;

            var existingEfEntity = await _context.ProductosProveedores
                .FirstOrDefaultAsync(x => x.IdProducto == idProducto && x.IdProveedor == idProveedor);

            if (existingEfEntity == null)
            {
                throw new KeyNotFoundException("Relación Producto-Proveedor no encontrada para actualización.");
            }

            existingEfEntity.Precio = entity.Precio;
            existingEfEntity.StockAsignado = entity.StockAsignado;

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

        public async Task<bool> ExistsAsync(int idProducto, int idProveedor)
        {
            return await _context.ProductosProveedores
                .AnyAsync(x => x.IdProducto == idProducto && x.IdProveedor == idProveedor);
        }
    }
