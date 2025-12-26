using src.Contracts;
using src.Core.Services.Interfaces;
using src.Models.Domain;
using src.Models.Mappers;
using src.Repositories.Interfaces;
using src.ViewModels;
using Dom = src.Models.Domain;
using src.Presentation.ViewModels.ProductoVM;
using ZXing.Maxicode;
using Microsoft.EntityFrameworkCore;
using Core.Common;

namespace src.Core.Services.Implementations;

public class ProductoProveedorService : IProductoProveedorService
{
    private readonly IProveedorRepository _proveedorRepository;
    private readonly IProductoProveedorRepository _productoProveedorRepository;
    private readonly IProductoCodigoExternoRepository _productoCodigoExternoRepository;
    private readonly IProductoRepository _productoRepository;

    public ProductoProveedorService(IProveedorRepository proveedorRepository, IProductoCodigoExternoRepository productoCodigoExternoRepository, IProductoRepository productoRepository, IProductoProveedorRepository productoProveedorRepository)
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
            StockAsignado = d.ProductoProveedor.StockAsignado,

            // Asignación directa de la lista (El ViewModel ahora espera List<string>)
            CodigosBarraExternos = d.CodigosBarrasExternos ?? new List<string>()
        });
    }


    public async Task<CrearProductoProveedorViewModel> PrepararCrearViewModelAsync()
    {
        var vm = new CrearProductoProveedorViewModel();
        await RepoblarViewModelAsync(vm);
        return vm;
    }

    public async Task<ActualizarProductoProveedorViewModel> PrepararActualizarViewModelAsync(int idProducto, int idProveedor)
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
            StockAsignado = dto.ProductoProveedor.StockAsignado,

            // Asignamos la lista para que se vea en la vista (como ReadOnly)
            CodigosBarraExternos = dto.CodigosBarrasExternos ?? new List<string>()
        };

        return vm;
    }

    public async Task<ServiceResult> CreateAsync(CrearProductoProveedorViewModel vm)
    {
        // // 1. Validar que no exista ya la relación
        // if (await _productoProveedorRepository.ExistsAsync(vm.IdProducto, vm.IdProveedor))
        // {
        //     throw new InvalidOperationException("Este producto ya está asignado a este proveedor.");
        // }
        // var codigosLimpios = new List<string>();
        // if (vm.CodigosBarraExternos != null && vm.CodigosBarraExternos.Any())
        // {
        //     // Filtramos nulos, vacíos y hacemos Trim
        //     codigosLimpios = vm.CodigosBarraExternos
        //         .Where(c => !string.IsNullOrWhiteSpace(c))
        //         .Select(c => c.Trim())
        //         .Distinct() // Evitamos duplicados en la misma carga
        //         .ToList();

        //     // 3. Validar unicidad global en la base de datos
        //     foreach (var codigo in codigosLimpios)
        //     {
        //         if (await _productoCodigoExternoRepository.ExistsAsync(codigo, (short)vm.IdProveedor))
        //         {
        //             throw new ArgumentException($"El código de barras '{codigo}' ya está asignado a otro producto en el sistema.");
        //         }
        //     }
        // }
        // // 2. Mapear al Dominio
        // var dominio = new Dom.ProductoProveedor
        // {
        //     // Usamos objetos dummy solo con el ID para que EF sepa las FKs
        //     Producto = new Dom.Producto { IdProducto = vm.IdProducto },
        //     Proveedor = new Dom.Proveedor { IdProveedor = vm.IdProveedor },
        //     Precio = vm.Precio,
        //     StockAsignado = vm.StockAsignado
        // };

        // await _productoProveedorRepository.AddAsync(dominio, codigosLimpios);
        var result = new ServiceResult();

        // 1. Validar que no exista ya la relación
        if (await _productoProveedorRepository.ExistsAsync(vm.IdProducto, vm.IdProveedor))
        {
            // Agregamos el error a los dos campos implicados para que se marquen en rojo
            result.AddError(nameof(vm.IdProducto), "Este producto ya está asignado a este proveedor.");
            result.AddError(nameof(vm.IdProveedor), "El proveedor ya tiene asignado este producto.");
        }

        // 2. Limpieza de códigos de barra
        var codigosLimpios = new List<string>();
        if (vm.CodigosBarraExternos != null && vm.CodigosBarraExternos.Any())
        {
            codigosLimpios = vm.CodigosBarraExternos
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim())
                .Distinct()
                .ToList();

            // 3. Validar unicidad global (Podemos capturar TODOS los códigos duplicados de una sola vez)
            foreach (var codigo in codigosLimpios)
            {
                if (await _productoCodigoExternoRepository.ExistsAsync(codigo, (short)vm.IdProveedor))
                {
                    // Agregamos un error específico por cada código fallido
                    result.AddError(nameof(vm.CodigosBarraExternos), $"El código '{codigo}' ya está asignado a otro producto.");
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
                StockAsignado = vm.StockAsignado
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

    public async Task UpdateAsync(ActualizarProductoProveedorViewModel vm)
    {
        // Validamos existencia antes de intentar mapear (opcional, el repo también lo hace)
        if (!await _productoProveedorRepository.ExistsAsync(vm.IdProducto, vm.IdProveedor))
        {
            throw new KeyNotFoundException("No se puede actualizar una relación inexistente.");
        }

        var dominio = new Dom.ProductoProveedor
        {
            // Claves compuestas necesarias para identificar el registro
            Producto = new Dom.Producto { IdProducto = vm.IdProducto },
            Proveedor = new Dom.Proveedor { IdProveedor = vm.IdProveedor },

            // Campos actualizables
            Precio = vm.Precio,
            StockAsignado = vm.StockAsignado
        };

        // El repositorio ya sabe que NO debe tocar el código de barras en el Update
        await _productoProveedorRepository.UpdateAsync(dominio);
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
        // 1. Obtener TODOS los productos del sistema (usando IProductoRepository)
        var productos = await _productoRepository.GetAllAsync();

        // 2. Obtener TODOS los proveedores (usando IProveedorRepository)
        var proveedores = await _proveedorRepository.GetAllProveedorAsync(); // Asegúrate que este método existe en tu interfaz

        vm.ListaProductos = productos
            .Where(p => p.Activo)
            .Select(p => new SelectListItemDto
            {
                Id = p.IdProducto,
                Descripcion = p.Nombre // + $" (Stock: {p.StockTotal})" // Opcional: agregar info extra
            })
            .OrderBy(x => x.Descripcion)
            .ToList();

        vm.ListaProveedores = proveedores
            .Where(p => p.Activo)
            .Select(p => new SelectListItemDto
            {
                Id = p.IdProveedor,
                Descripcion = p.RazonSocial
            })
            .OrderBy(x => x.Descripcion)
            .ToList();
    }
}