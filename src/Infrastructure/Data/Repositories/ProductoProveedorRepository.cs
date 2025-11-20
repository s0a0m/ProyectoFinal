using Microsoft.EntityFrameworkCore;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;
using src.Repositories.Interfaces;
using src.Models.Mappers;
using src.Models.Domain;

namespace src.Repositories.Implementations;

public class ProductoProveedorRepository : IProductoProveedorRepository
{
    private readonly EF.AppDbContext _context;

    public ProductoProveedorRepository(EF.AppDbContext context)
    {
        _context = context;
    }

    private IQueryable<EF.ProductoProveedor> GetQueryProveedor()
    {
        return _context.ProductosProveedores
        .Include(p => p.Producto)
        .Include(p => p.Proveedor);
    }

    public async Task UpdateAsync(ProductoProveedor entity)
    {

    }
}