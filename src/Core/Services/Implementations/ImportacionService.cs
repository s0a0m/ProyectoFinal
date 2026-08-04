using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using src.Contracts;
using src.Core.Services.Interfaces;
using src.Models.CodeFirst;
using src.Models.Common;
using src.Presentation.ViewModels.FamiliaVM;
using src.Presentation.ViewModels.NovedadesVM;
using src.Repositories.Interfaces;
using Dom = src.Models.Domain;

namespace src.Core.Services.Implementations
{
    public class ImportacionService : IImportacionService
    {
        private readonly IExcelDataReader _excelReader;
        private readonly INovedadesRepository _novedadesRepository;
        private readonly IProductoCodigoExternoRepository _prodCodigoExtRepo;
        private readonly IProductoProveedorRepository _prodProvRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBarcodeAdapter _barcodeAdapter;

        public ImportacionService(
            IExcelDataReader excelReader,
            INovedadesRepository novedadesRepository,
            IProductoCodigoExternoRepository prodCodigoExtRepo,
            IProductoProveedorRepository prodProvRepo,
            IUnitOfWork unitOfWork,
            IBarcodeAdapter barcodeAdapter
        )
        {
            _excelReader = excelReader;
            _novedadesRepository = novedadesRepository;
            _prodCodigoExtRepo = prodCodigoExtRepo;
            _prodProvRepo = prodProvRepo;
            _unitOfWork = unitOfWork;
            _barcodeAdapter = barcodeAdapter;
        }

        public async IAsyncEnumerable<AccionDeFilaCargaAutomatica> ProcesarListaDePreciosAsync(
            Stream fileStream,
            short idProveedor,
            ImportacionColumnaMap mapaColumnas,
            bool contieneEncabezado,
            CancellationToken cancellationToken
        )
        {
            const int TAMANO_LOTE = 200;
            var filasBrutas = _excelReader.ReadDataAsync(
                fileStream,
                mapaColumnas,
                contieneEncabezado,
                cancellationToken
            );
            var loteFilas = new List<ProductoProveedorDataRow>();

            foreach (var fila in filasBrutas)
            {
                if (string.IsNullOrWhiteSpace(fila.CodigoBarraExterno))
                {
                    yield return new AccionDeFilaCargaAutomatica
                    {
                        Accion = "Fila Ignorada: Sin Código",
                        Nombre = fila.NombreSugerido,
                    };
                    continue;
                }

                loteFilas.Add(fila);

                if (loteFilas.Count >= TAMANO_LOTE)
                {
                    await foreach (
                        var resultado in ProcesarLoteAsync(
                            loteFilas,
                            idProveedor,
                            cancellationToken
                        )
                    )
                    {
                        yield return resultado;
                    }
                    loteFilas.Clear();
                }
            }

            if (loteFilas.Any())
            {
                await foreach (
                    var resultado in ProcesarLoteAsync(loteFilas, idProveedor, cancellationToken)
                )
                {
                    yield return resultado;
                }
            }
        }

        private async IAsyncEnumerable<AccionDeFilaCargaAutomatica> ProcesarLoteAsync(
            List<ProductoProveedorDataRow> filas,
            short idProveedor,
            CancellationToken ct
        )
        {
            var codigosDelLote = filas.Select(f => f.CodigoBarraExterno).Distinct().ToList();

            var productosExistentes = await _prodCodigoExtRepo.ObtenerDiccionarioPorCodigosAsync(
                codigosDelLote,
                idProveedor,
                ct
            );
            var novedadesPendientes = await _novedadesRepository.ObtenerPendientesPorCodigosAsync(
                codigosDelLote,
                idProveedor,
                ct
            );

            var resultadosDelLote = new List<AccionDeFilaCargaAutomatica>();

            foreach (var fila in filas)
            {
                var resultado = new AccionDeFilaCargaAutomatica
                {
                    CodigoBarra = fila.CodigoBarraExterno,
                    Nombre = fila.NombreSugerido,
                    EsGs1 = _barcodeAdapter.ValidarFormatoGS1EAN13(fila.CodigoBarraExterno),
                };

                if (fila.Precio > 99999999.99m || fila.Precio < 0)
                {
                    resultado.Accion = "Error: Precio fuera de rango válido";
                    resultadosDelLote.Add(resultado);
                    continue;
                }

                // if (fila.StockActual > 99999999 || fila.StockActual < 0)
                // {
                //     resultado.Accion = "Error: Stock fuera de rango válido";
                //     resultadosDelLote.Add(resultado);
                //     continue;
                // }

                // CASO 1: ¿El producto ya existe y está vinculado al proveedor?
                if (
                    productosExistentes.TryGetValue(
                        fila.CodigoBarraExterno,
                        out var productoProvExistente
                    )
                )
                {
                    productoProvExistente.Precio = fila.Precio;
                    // productoProvExistente.StockAsignado = fila.StockActual;
                    // productoProvExistente.FechaActualizacion = DateTime.UtcNow; // Si tienes este campo

                    resultado.Accion = "Producto Actualizado: Precio modificado";
                    resultado.Nombre =
                        productoProvExistente.Producto?.Nombre ?? fila.NombreSugerido;
                }
                // CASO 2: ¿Ya existe una Novedad PENDIENTE para este código?
                else if (
                    novedadesPendientes.TryGetValue(
                        fila.CodigoBarraExterno,
                        out var novedadExistente
                    )
                )
                {
                    bool huboCambio = false;

                    if (novedadExistente.PrecioSugerido != fila.Precio)
                    {
                        novedadExistente.PrecioSugerido = fila.Precio;
                        huboCambio = true;
                    }
                    // if (novedadExistente.StockSugerido != fila.StockActual)
                    // {
                    //     novedadExistente.StockSugerido = fila.StockActual;
                    //     huboCambio = true;
                    // }

                    if (huboCambio)
                    {
                        novedadExistente.FechaModificacion = DateTime.UtcNow;
                        novedadExistente.Observaciones = "Actualizado por re-carga de Excel";
                        resultado.Accion = "Novedad Repetida: Datos actualizados por Excel";
                    }
                    else
                    {
                        resultado.Accion = "Fila ignorada: Novedad duplicada";
                    }
                }
                // CASO 3: Es totalmente nuevo -> Crear Novedad
                else
                {
                    var nuevaNovedad = new NovedadesProveedor
                    {
                        IdProveedor = idProveedor,
                        CodigoBarraExterno = fila.CodigoBarraExterno,
                        NombreSugerido = fila.NombreSugerido,
                        PrecioSugerido = fila.Precio,
                        // StockSugerido = fila.StockActual,
                        Estado = EstadoNovedad.PENDIENTE,
                        FechaImportacion = DateTime.UtcNow,
                    };

                    await _novedadesRepository.AddSinGuardarAsync(nuevaNovedad, ct);
                    novedadesPendientes[fila.CodigoBarraExterno] = nuevaNovedad;
                    resultado.Accion = "Novedad Creada: continuar en novedades";
                }

                resultadosDelLote.Add(resultado);
            }

            bool guardadoExitoso = false;
            string mensajeError = string.Empty;

            try
            {
                await _unitOfWork.SaveChangesAsync(ct);
                guardadoExitoso = true;
            }
            catch (Exception ex)
            {
                guardadoExitoso = false;
                mensajeError = ex.InnerException?.Message ?? ex.Message;
                _unitOfWork.LimpiarRastreador();
            }

            foreach (var res in resultadosDelLote)
            {
                if (!guardadoExitoso)
                {
                    res.Accion = $"ERROR EN LOTE: {mensajeError}";
                }

                yield return res;
            }
        }
    }
}
