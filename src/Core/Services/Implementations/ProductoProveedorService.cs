using Core.Common;
using Microsoft.EntityFrameworkCore;
using src.Contracts;
using src.Core.Services.Interfaces;
using src.Models.Domain;
using src.Models.Mappers;
using src.Presentation.ViewModels.ProductoVM;
using src.Repositories.Interfaces;
using src.ViewModels;
using ZXing.Maxicode;
using Dom = src.Models.Domain;

namespace src.Core.Services.Implementations;

public class ProductoProveedorService : IProductoProveedorService
{
    private readonly IProveedorRepository _proveedorRepository;
    private readonly IProductoProveedorRepository _productoProveedorRepository;
    private readonly IProductoCodigoExternoRepository _productoCodigoExternoRepository;
    private readonly IProductoRepository _productoRepository;

    public ProductoProveedorService(
        IProveedorRepository proveedorRepository,
        IProductoCodigoExternoRepository productoCodigoExternoRepository,
        IProductoRepository productoRepository,
        IProductoProveedorRepository productoProveedorRepository
    )
    {
        _proveedorRepository = proveedorRepository;
        this._productoCodigoExternoRepository = productoCodigoExternoRepository;
        this._productoProveedorRepository = productoProveedorRepository;
        _productoRepository = productoRepository;
    }

    public async Task<IEnumerable<ProductoProveedorListarViewModel>> GetAllParaListadoAsync()
    {
        // El repo ahora devuelve ProductoProveedorDto con List<string> CodigosBarrasExternos
        var dtos = await _productoProveedorRepository.GetAllAsync();

        return dtos.Select(d => new ProductoProveedorListarViewModel
        {
            IdProducto = d.ProductoProveedor.Producto.IdProducto,
            IdProveedor = d.ProductoProveedor.Proveedor.IdProveedor,
            NombreProducto = d.ProductoProveedor.Producto.Nombre,
            NombreProveedor = d.ProductoProveedor.Proveedor.RazonSocial,
            Precio = d.ProductoProveedor.Precio,
            // StockAsignado = d.ProductoProveedor.StockAsignado,

            // Asignación directa de la lista (El ViewModel ahora espera List<string>)
            CodigosBarraExternos = d.CodigosBarrasExternos ?? new List<string>(),
        });
    }

    public async Task<CrearProductoProveedorViewModel> PrepararCrearViewModelAsync()
    {
        var vm = new CrearProductoProveedorViewModel();
        await RepoblarViewModelAsync(vm);
        return vm;
    }

    public async Task<ActualizarProductoProveedorViewModel> PrepararActualizarViewModelAsync(
        int idProducto,
        int idProveedor
    )
    {
        var dto = await _productoProveedorRepository.GetByIdAsync(idProducto, idProveedor);

        if (dto == null)
            throw new KeyNotFoundException("La relación Producto-Proveedor no fue encontrada.");

        var vm = new ActualizarProductoProveedorViewModel
        {
            IdProducto = dto.ProductoProveedor.Producto.IdProducto,
            IdProveedor = dto.ProductoProveedor.Proveedor.IdProveedor,

            // Campos informativos
            NombreProducto = dto.ProductoProveedor.Producto.Nombre,
            RazonSocialProveedor = dto.ProductoProveedor.Proveedor.RazonSocial,

            // Campos editables
            Precio = dto.ProductoProveedor.Precio,
            // StockAsignado = dto.ProductoProveedor.StockAsignado,

            // Asignamos la lista para que se vea en la vista (como ReadOnly)
            CodigosBarraExternos = dto.CodigosBarrasExternos ?? new List<string>(),
        };

        return vm;
    }

    public async Task<ServiceResult> CreateAsync(CrearProductoProveedorViewModel vm)
    {
        var result = new ServiceResult();

        if (await _productoProveedorRepository.ExistsAsync(vm.IdProducto, vm.IdProveedor))
        {
            result.AddError(
                nameof(vm.IdProducto),
                "Este producto ya está asignado a este proveedor."
            );
            result.AddError(
                nameof(vm.IdProveedor),
                "El proveedor ya tiene asignado este producto."
            );
        }

        var codigosLimpios = new List<string>();
        if (vm.CodigosBarraExternos != null && vm.CodigosBarraExternos.Any())
        {
            codigosLimpios = vm
                .CodigosBarraExternos.Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim())
                .Distinct()
                .ToList();

            foreach (var codigo in codigosLimpios)
            {
                if (
                    await _productoCodigoExternoRepository.ExistsAsync(
                        codigo,
                        (short)vm.IdProveedor
                    )
                )
                {
                    result.AddError(
                        nameof(vm.CodigosBarraExternos),
                        $"El código '{codigo}' ya está asignado a otro producto del proveedor."
                    );
                }
            }
        }

        // SI HAY ERRORES, cortamos aquí y devolvemos la "bolsa de errores"
        if (!result.Success)
        {
            return result;
        }

        try
        {
            // 4. Mapear al Dominio
            var dominio = new Dom.ProductoProveedor
            {
                Producto = new Dom.Producto { IdProducto = vm.IdProducto },
                Proveedor = new Dom.Proveedor { IdProveedor = vm.IdProveedor },
                Precio = vm.Precio,
                // StockAsignado = vm.StockAsignado
            };

            await _productoProveedorRepository.AddAsync(dominio, codigosLimpios);

            return ServiceResult.Ok("Relación creada correctamente.");
        }
        catch (Exception ex)
        {
            // Solo usamos Fail para errores técnicos inesperados (Base de datos caída, etc.)
            return ServiceResult.Fail($"Error interno al guardar: {ex.Message}");
        }
    }

    public async Task<ServiceResult> UpdateAsync(ActualizarProductoProveedorViewModel vm)
    {
        var result = new ServiceResult();

        if (!await _productoProveedorRepository.ExistsAsync(vm.IdProducto, vm.IdProveedor))
            throw new KeyNotFoundException("No se puede actualizar una relación inexistente.");

        // Limpiar y validar los nuevos códigos
        var nuevosCodigosLimpios = new List<string>();
        if (vm.NuevosCodigosBarraExternos != null && vm.NuevosCodigosBarraExternos.Any())
        {
            nuevosCodigosLimpios = vm
                .NuevosCodigosBarraExternos.Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim())
                .Distinct()
                .ToList();

            foreach (var codigo in nuevosCodigosLimpios)
            {
                if (
                    await _productoCodigoExternoRepository.ExistsAsync(
                        codigo,
                        (short)vm.IdProveedor
                    )
                )
                {
                    result.AddError(
                        nameof(vm.NuevosCodigosBarraExternos),
                        $"El código '{codigo}' ya está asignado a otro producto del proveedor."
                    );
                }
            }
        }

        if (!result.Success)
            return result;

        try
        {
            var dominio = new Dom.ProductoProveedor
            {
                Producto = new Dom.Producto { IdProducto = vm.IdProducto },
                Proveedor = new Dom.Proveedor { IdProveedor = vm.IdProveedor },
                Precio = vm.Precio,
                // StockAsignado = vm.StockAsignado
            };

            await _productoProveedorRepository.UpdateAsync(dominio, nuevosCodigosLimpios);

            return ServiceResult.Ok("Relación actualizada correctamente.");
        }
        catch (Exception ex)
        {
            return ServiceResult.Fail($"Error interno al guardar: {ex.Message}");
        }
    }

    public async Task GestionarCascadaProductoAsync(int idProducto, bool activando)
    {
        if (activando)
        {
            // El producto se reactivó -> Intentamos reactivar sus relaciones (validando proveedores)
            await _productoProveedorRepository.ReactivarPorProductoAsync(idProducto);
        }
        else
        {
            // El producto se desactivó -> Apagamos todas sus relaciones
            await _productoProveedorRepository.DesactivarPorProductoAsync(idProducto);
        }
    }

    public async Task GestionarCascadaProveedorAsync(int idProveedor, bool activando)
    {
        if (activando)
        {
            // El proveedor se reactivó -> Intentamos reactivar sus relaciones (validando productos)
            await _productoProveedorRepository.ReactivarPorProveedorAsync(idProveedor);
        }
        else
        {
            // El proveedor se desactivó -> Apagamos todas sus relaciones
            await _productoProveedorRepository.DesactivarPorProveedorAsync(idProveedor);
        }
    }

    // Método para llenar los Combos/Selects de la Vista
    public async Task RepoblarViewModelAsync(CrearProductoProveedorViewModel vm)
    {
        var productos = await _productoRepository.GetAllAsync();

        var proveedores = await _proveedorRepository.GetAllProveedorAsync();

        vm.ListaProductos = productos
            .Where(p => p.Activo)
            .Select(p => new SelectListItemDto { Id = p.IdProducto, Descripcion = p.Nombre })
            .OrderBy(x => x.Descripcion)
            .ToList();

        vm.ListaProveedores = proveedores
            .Where(p => p.Activo)
            .Select(p => new SelectListItemDto { Id = p.IdProveedor, Descripcion = p.RazonSocial })
            .OrderBy(x => x.Descripcion)
            .ToList();
    }
}

