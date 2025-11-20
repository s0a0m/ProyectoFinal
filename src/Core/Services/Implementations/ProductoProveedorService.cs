using src.Contracts;
using src.Core.Services.Interfaces;
using src.Models.Domain;
using src.Models.Mappers;
using src.Repositories.Interfaces;
using src.ViewModels;
using Dom = src.Models.Domain;

namespace src.Core.Services.Implementations;

public class ProductoProveedorService : IProductoProveedorService
{
    private readonly IProveedorRepository _proveedorRepository;
    private readonly IProductoProveedorRepository _productoProveedorRepository;
    private readonly INovedadesRepository _novedadesRepository;
    private readonly IProductoCodigoExternoRepository _productoCodigoExternoRepository;
    private readonly IExcelDataReader _excelReader;
    private readonly IBarcodeAdapter _barcodeAdapter;

    public ProductoProveedorService(IExcelDataReader reader, IProveedorRepository proveedorRepository, ICommonDataService _commonDataService, IProductoCodigoExternoRepository productoCodigoExternoRepository, IProductoProveedorRepository productoProveedorRepository, IBarcodeAdapter barcodeAdapter, INovedadesRepository novedadesRepository)
    {
        _proveedorRepository = proveedorRepository;
        this._excelReader = reader;
        this._productoCodigoExternoRepository = productoCodigoExternoRepository;
        this._productoProveedorRepository = productoProveedorRepository;
        _barcodeAdapter = barcodeAdapter;
        _novedadesRepository = novedadesRepository;
    }

    public async Task<IEnumerable<AccionDeFilaCargaAutomatica>> ProcesarListaDePreciosAsync(Stream fileStream, short idProveedor, ImportacionColumnaMap mapaColumnas)
    {
        var resultadosAccion = new List<AccionDeFilaCargaAutomatica>();
        IEnumerable<ProductoProveedorDataRow> filasBrutas = await _excelReader.ReadDataAsync(fileStream, mapaColumnas);

        foreach (var fila in filasBrutas)
        {
            if (string.IsNullOrWhiteSpace(fila.CodigoBarraExterno)) continue;
            // 1. ver si alguno de los fila.CodigoBarraExterno esta asociado con algun producto (ProductoCodigoExterno) 
            var productoAsociado = await _productoCodigoExternoRepository.ObtenerProductoPorCodigoAsync(fila.CodigoBarraExterno, idProveedor);

            // 1. SI
            AccionDeFilaCargaAutomatica accion;
            if (productoAsociado is not null)
            {
                // 2. actualizar valores (ProductoProveedor) ignora nombre
                ProductoProveedor domproductoProveedor = DominioMapper.Map(fila);
                await _productoProveedorRepository.UpdateAsync(domproductoProveedor);
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
                await _novedadesRepository.AddAsync(novedad);
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
            resultadosAccion.Add(accion);
        }
        return resultadosAccion;
    }

}