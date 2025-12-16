using src.Contracts;
using src.Core.Services.Interfaces;
using src.Models.Domain;
using src.Models.Mappers;
using src.Repositories.Interfaces;
using src.ViewModels;
using Dom = src.Models.Domain;
using src.Presentation.ViewModels.ProductoVM;
using ZXing.Maxicode;

namespace src.Core.Services.Implementations;

public class ProductoProveedorService : IProductoProveedorService
{
    private readonly IProveedorRepository _proveedorRepository;
    private readonly IProductoProveedorRepository _productoProveedorRepository;
    private readonly INovedadesRepository _novedadesRepository;
    private readonly IProductoCodigoExternoRepository _productoCodigoExternoRepository;
    private readonly IExcelDataReader _excelReader;
    private readonly IBarcodeAdapter _barcodeAdapter;
    private readonly IProductoRepository _productoRepository;

    public ProductoProveedorService(IExcelDataReader reader, IProveedorRepository proveedorRepository, ICommonDataService _commonDataService, IProductoCodigoExternoRepository productoCodigoExternoRepository, IProductoRepository productoRepository, IProductoProveedorRepository productoProveedorRepository, IBarcodeAdapter barcodeAdapter, INovedadesRepository novedadesRepository)
    {
        _proveedorRepository = proveedorRepository;
        this._excelReader = reader;
        this._productoCodigoExternoRepository = productoCodigoExternoRepository;
        this._productoProveedorRepository = productoProveedorRepository;
        _barcodeAdapter = barcodeAdapter;
        _novedadesRepository = novedadesRepository;
        _productoRepository = productoRepository;
    }

    public async IAsyncEnumerable<AccionDeFilaCargaAutomatica> ProcesarListaDePreciosAsync(Stream fileStream, short idProveedor, ImportacionColumnaMap mapaColumnas, bool contieneEncabezado = true, CancellationToken cancellationToken = default)
    {
        // const int MAX_FILAS_PERMITIDAS = 3000000;

        // int totalFilasDeDatos = _excelReader.GetRowsCount(fileStream);

        // if (totalFilasDeDatos > MAX_FILAS_PERMITIDAS)
        // {
        //     throw new ArgumentException($"El archivo excede el límite de {MAX_FILAS_PERMITIDAS} filas de datos. Por favor, divida el archivo e inténtelo de nuevo.");
        // }

        if (await _proveedorRepository.GetProveedorById(idProveedor) == null)
        {
            throw new KeyNotFoundException($"El proveedor con ID {idProveedor} no existe.");
        }

        if (!_excelReader.EsArchivoExcelValido(fileStream))
        {
            throw new InvalidOperationException("El archivo Excel está corrupto o protegido con contraseña y no puede ser procesado.");
        }

        var indicesMapeadosValidos = new List<int>();

        // Esta lista incluirá todos los campos que el usuario haya seleccionado.
        if (mapaColumnas.CodigosBarrasExternosIndex >= 0)
            indicesMapeadosValidos.Add(mapaColumnas.CodigosBarrasExternosIndex);

        if (mapaColumnas.PrecioIndex >= 0)
            indicesMapeadosValidos.Add(mapaColumnas.PrecioIndex);

        if (mapaColumnas.NombreSugeridoIndex >= 0)
            indicesMapeadosValidos.Add(mapaColumnas.NombreSugeridoIndex);

        if (mapaColumnas.StockIndex >= 0)
            indicesMapeadosValidos.Add(mapaColumnas.StockIndex);

        if (indicesMapeadosValidos.Count == 0)
        {
            throw new ArgumentException("La lista de precios no puede ser procesada. Debe mapear al menos una columna de datos (ej. Código de Barras, Precio o Nombre).");
        }

        var indicesUnicos = new HashSet<int>(indicesMapeadosValidos);

        if (indicesUnicos.Count < indicesMapeadosValidos.Count)
        {
            throw new ArgumentException("Error en el mapeo: Se asignaron múltiples campos de destino a la misma columna de Excel. Por favor, corrija el mapeo.");
        }

        IEnumerable<ProductoProveedorDataRow> filasBrutas = _excelReader.ReadDataAsync(fileStream, mapaColumnas, contieneEncabezado, cancellationToken);

        foreach (var fila in filasBrutas)
        {
            cancellationToken.ThrowIfCancellationRequested();

            AccionDeFilaCargaAutomatica accion;
            if (string.IsNullOrWhiteSpace(fila.CodigoBarraExterno))
            {
                accion = new AccionDeFilaCargaAutomatica
                {
                    Nombre = fila.NombreSugerido,
                    CodigoBarra = fila.CodigoBarraExterno,
                    EsGs1 = false,
                    Accion = "Fila ignorada: Código de barras vacío."
                };
                yield return accion;
                continue;
            }
            // 1. ver si alguno de los fila.CodigoBarraExterno esta asociado con algun producto (ProductoCodigoExterno) 
            var productoAsociado = await _productoCodigoExternoRepository.ObtenerProductoPorCodigoAsync(fila.CodigoBarraExterno, idProveedor, cancellationToken);

            // 1. SI
            if (productoAsociado is not null)
            {
                // 2. actualizar valores (ProductoProveedor) ignora nombre
                ProductoProveedor domProductoProveedor = DominioMapper.Map(fila);
                domProductoProveedor.Precio = fila.Precio;
                domProductoProveedor.Producto = new Dom.Producto { IdProducto = productoAsociado.IdProducto };
                domProductoProveedor.Proveedor = new Dom.Proveedor { IdProveedor = idProveedor };
                domProductoProveedor.StockAsignado = fila.StockActual;
                await _productoProveedorRepository.UpdateAsync(domProductoProveedor, cancellationToken);
                accion = new AccionDeFilaCargaAutomatica
                {
                    Nombre = productoAsociado.Nombre,
                    CodigoBarra = fila.CodigoBarraExterno,
                    EsGs1 = _barcodeAdapter.ValidarFormatoGS1EAN13(fila.CodigoBarraExterno),
                    Accion = "Actualización de Precio"
                };
            }
            else
            {
                // 1. NO 
                // 2. Mappear con NovedadesProveedor (ProductoIdProcuto = 0, Estado = PENDIENTE)
                var novedad = DominioMapper.MapDataRow(fila);
                novedad.IdNovedad = 0;
                novedad.IdProveedor = idProveedor;
                novedad.Estado = Models.Common.EstadoNovedad.PENDIENTE;

                // agregar novedad
                await _novedadesRepository.AddAsync(novedad, cancellationToken);
                // TODO: ver si el codigo es GS1 y ponerle una flag
                bool esGs1 = _barcodeAdapter.ValidarFormatoGS1EAN13(fila.CodigoBarraExterno);
                // Retornar una clase con (codigo y Nombre del producto y ACCION (guardado, novedad))
                accion = new AccionDeFilaCargaAutomatica
                {
                    Nombre = novedad.NombreSugerido,
                    CodigoBarra = novedad.CodigoBarraExterno,
                    EsGs1 = esGs1,
                    Accion = $"Novedad Creada (Calidad: {(esGs1 ? "GS1" : "SKU")})"
                };
            }
            yield return accion;
        }
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

    public async Task CreateAsync(CrearProductoProveedorViewModel vm)
    {
        // 1. Validar que no exista ya la relación
        if (await _productoProveedorRepository.ExistsAsync(vm.IdProducto, vm.IdProveedor))
        {
            throw new InvalidOperationException("Este producto ya está asignado a este proveedor.");
        }
        var codigosLimpios = new List<string>();
        if (vm.CodigosBarraExternos != null && vm.CodigosBarraExternos.Any())
        {
            // Filtramos nulos, vacíos y hacemos Trim
            codigosLimpios = vm.CodigosBarraExternos
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim())
                .Distinct() // Evitamos duplicados en la misma carga
                .ToList();

            // 3. Validar unicidad global en la base de datos
            foreach (var codigo in codigosLimpios)
            {
                if (await _productoCodigoExternoRepository.ExistsAsync(codigo))
                {
                    throw new ArgumentException($"El código de barras '{codigo}' ya está asignado a otro producto en el sistema.");
                }
            }
        }
        // 2. Mapear al Dominio
        var dominio = new Dom.ProductoProveedor
        {
            // Usamos objetos dummy solo con el ID para que EF sepa las FKs
            Producto = new Dom.Producto { IdProducto = vm.IdProducto },
            Proveedor = new Dom.Proveedor { IdProveedor = vm.IdProveedor },
            Precio = vm.Precio,
            StockAsignado = vm.StockAsignado
        };

        await _productoProveedorRepository.AddAsync(dominio, codigosLimpios);
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