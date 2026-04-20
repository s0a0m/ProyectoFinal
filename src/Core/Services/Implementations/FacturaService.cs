using Core.Common;
using src.Core.Contracts;
using src.Core.Services.Interfaces;
using src.Models.Common;
using src.Repositories.Interfaces;
using Dom = src.Models.Domain;

namespace src.Core.Services.Implementations;

public class FacturaService : IFacturaService
{
    private readonly IFacturaRepository _facturaRepository;
    private readonly ICondicionPagoRepository _condicionPagoRepository;
    private readonly ICompraRepository _compraRepository;
    private readonly IComprobanteRepository _comprobanteRepository;
    private readonly IOrdenPagoRepository _ordenPagoRepository;
    private readonly IUnitOfWork _uow;
    private readonly IProveedorRepository _proveedorRepository;

    public FacturaService(
        IFacturaRepository facturaRepo,
        ICondicionPagoRepository condicionPagoRepository,
        ICompraRepository compraRepository,
        IComprobanteRepository comprobanteRepository,
        IOrdenPagoRepository ordenPagoRepository,
        IUnitOfWork uow,
        IProveedorRepository proveedorRepository
    )
    {
        _facturaRepository = facturaRepo;
        _condicionPagoRepository = condicionPagoRepository;
        _compraRepository = compraRepository;
        _comprobanteRepository = comprobanteRepository;
        _ordenPagoRepository = ordenPagoRepository;
        _uow = uow;
        _proveedorRepository = proveedorRepository;
    }

    public async Task<IEnumerable<Dom.Factura>> ObtenerTodasAsync()
    {
        return await _facturaRepository.GetAllAsync();
    }

    public async Task<ServiceResult<Dom.Factura>> ObtenerPorIdAsync(int id)
    {
        var factura = await _facturaRepository.GetByIdAsync(id);
        if (factura == null)
            return ServiceResult<Dom.Factura>.Fail("La factura solicitada no existe.");

        return ServiceResult<Dom.Factura>.Ok(factura);
    }

    public async Task<IEnumerable<Dom.Factura>> ObtenerPendientesPagoAsync()
    {
        return await _facturaRepository.GetPendientesPagoAsync();
    }

    public async Task<ServiceResult<Dom.Compra>> ValidarCompraParaFacturacionAsync(int idCompra)
    {
        var result = await _compraRepository.GetByIdAsync(idCompra);

        if (result == null)
            return ServiceResult<Dom.Compra>.Fail("La orden de compra no existe.");

        if (result.Estado != EstadoCompra.ENVIADA)
            return ServiceResult<Dom.Compra>.Fail(
                $"No se puede facturar: la orden está en estado {result.Estado}. Solo se permiten órdenes ENVIADAS."
            );

        return ServiceResult<Dom.Compra>.Ok(result);
    }

    public async Task<bool> ExisteNumeroFacturaAsync(short idProveedor, string numero)
    {
        return await _facturaRepository.ExisteNumeroFacturaAsync(idProveedor, numero);
    }

    public async Task<ServiceResult<DocumentosRelacionadosData>> ObtenerDocumentosAsociadosAsync(
        int idFactura
    )
    {
        var factura = await _facturaRepository.GetByIdAsync(idFactura);
        if (factura == null)
            return ServiceResult<DocumentosRelacionadosData>.Fail(
                "No se pueden obtener documentos de una factura inexistente."
            );

        var comprobantes = await _comprobanteRepository.GetByFacturaIdAsync(idFactura);
        var ordenes = await _ordenPagoRepository.GetPagosPorFacturaIdAsync(idFactura);

        var data = new DocumentosRelacionadosData
        {
            Factura = factura,
            Comprobantes = comprobantes.ToList(),
            OrdenesPago = ordenes.ToList(),
        };

        return ServiceResult<DocumentosRelacionadosData>.Ok(data);
    }

    public async Task<ServiceResult<Dom.Compra>> ObtenerCompraParaFacturarAsync(int idCompra)
    {
        var compra = await _compraRepository.GetByIdAsync(idCompra);

        if (compra == null)
            return ServiceResult<Dom.Compra>.Fail("La orden de compra no existe.");

        if (compra.Estado != EstadoCompra.ENVIADA)
            return ServiceResult<Dom.Compra>.Fail(
                $"No se puede facturar: la orden está en estado {compra.Estado}. Solo se permiten órdenes ENVIADAS."
            );

        return ServiceResult<Dom.Compra>.Ok(compra);
    }

    public async Task<ServiceResult<int>> CrearDesdeCompraAsync(Dom.Factura factura, int idCompra)
    {
        try
        {
            await _uow.BeginTransactionAsync();

            var compraResult = await ObtenerCompraParaFacturarAsync(idCompra);
            if (!compraResult.Success)
            {
                await _uow.RollbackAsync();
                return ServiceResult<int>.Fail(compraResult.Message);
            }

            decimal total = factura.Detalles.Sum(d => d.Cantidad * d.PrecioNeto);
            if (total > BusinessLimits.MAX_TOTAL_COMPRA)
            {
                await _uow.RollbackAsync();
                return ServiceResult<int>.Fail(
                    "El monto total de la factura excede el límite permitido."
                );
            }

            bool existe = await _facturaRepository.ExisteNumeroFacturaAsync(
                (short)factura.Proveedor.IdProveedor,
                factura.Numero
            );
            if (existe)
            {
                await _uow.RollbackAsync();
                return ServiceResult<int>.Fail(
                    "Este número de factura ya fue registrado para este proveedor."
                );
            }

            factura.TotalFacturado = total;
            factura.Saldo = total;
            factura.Pagada = false;

            if (factura.CondicionPago != null)
            {
                factura.CondicionPago = await _condicionPagoRepository.BuscarOCrearAsync(
                    factura.CondicionPago
                );
            }

            var proveedor = await _proveedorRepository.GetProveedorById(
                factura.Proveedor.IdProveedor
            );
            if (proveedor == null)
            {
                await _uow.RollbackAsync();
                return ServiceResult<int>.Fail("Proveedor no encontrado.");
            }

            proveedor.AumentarSaldo(factura.TotalFacturado);
            await _proveedorRepository.UpdateAsync(proveedor);

            var facturaCreada = await _facturaRepository.AddAsync(factura);
            await _compraRepository.CompletarCompraAsync(idCompra);

            await _uow.CommitAsync();
            return ServiceResult<int>.Ok(
                facturaCreada.IdFactura,
                "Factura registrada exitosamente."
            );
        }
        catch (Exception ex)
        {
            await _uow.RollbackAsync();
            return ServiceResult<int>.Fail($"Error al procesar la factura: {ex.Message}");
        }
    }

    public async Task<ServiceResult> ActualizarAsync(Dom.Factura facturaEditada)
    {
        try
        {
            await _uow.BeginTransactionAsync();

            var facturaDB = await _facturaRepository.GetByIdAsync(facturaEditada.IdFactura);
            if (facturaDB == null)
                return ServiceResult.Fail("Factura no encontrada.");

            if (facturaDB.Saldo < facturaDB.TotalFacturado || facturaDB.Pagada)
                return ServiceResult.Fail(
                    "No se puede modificar una factura que ya posee pagos o créditos asociados."
                );

            if (facturaEditada.Numero != facturaDB.Numero)
            {
                bool existe = await _facturaRepository.ExisteNumeroFacturaAsync(
                    (short)facturaDB.Proveedor.IdProveedor,
                    facturaEditada.Numero,
                    facturaDB.IdFactura
                );

                if (existe)
                {
                    await _uow.RollbackAsync();
                    return ServiceResult.Fail(
                        "El nuevo número de factura ya existe para este proveedor."
                    );
                }
            }

            decimal totalViejo = facturaDB.TotalFacturado;
            facturaDB.Numero = facturaEditada.Numero;
            facturaDB.FechaEmision = facturaEditada.FechaEmision;

            if (facturaEditada.CondicionPago != null)
            {
                facturaDB.CondicionPago = await _condicionPagoRepository.BuscarOCrearAsync(
                    facturaEditada.CondicionPago
                );
            }

            foreach (var detalleDB in facturaDB.Detalles)
            {
                var editado = facturaEditada.Detalles.FirstOrDefault(d =>
                    d.Producto.IdProducto == detalleDB.Producto.IdProducto
                );
                if (editado != null)
                {
                    detalleDB.PorcentajeDescuento = editado.PorcentajeDescuento;
                    detalleDB.PrecioNeto =
                        detalleDB.PrecioBruto
                        - (detalleDB.PrecioBruto * (detalleDB.PorcentajeDescuento / 100));
                }
            }

            decimal nuevoTotal = facturaDB.Detalles.Sum(d => d.Cantidad * d.PrecioNeto);

            if (nuevoTotal > BusinessLimits.MAX_TOTAL_COMPRA)
            {
                await _uow.RollbackAsync();
                return ServiceResult.Fail("El nuevo monto total excede el límite permitido.");
            }

            facturaDB.TotalFacturado = nuevoTotal;
            facturaDB.Saldo = nuevoTotal;

            var proveedor = await _proveedorRepository.GetProveedorById(
                facturaDB.Proveedor.IdProveedor
            );
            if (proveedor != null)
            {
                proveedor.SaldoActual = (proveedor.SaldoActual - totalViejo) + nuevoTotal;
                await _proveedorRepository.UpdateAsync(proveedor);
            }

            await _facturaRepository.UpdateAsync(facturaDB);
            await _uow.CommitAsync();

            return ServiceResult.Ok("Factura actualizada correctamente.");
        }
        catch (Exception ex)
        {
            await _uow.RollbackAsync();
            return ServiceResult.Fail($"Error al actualizar: {ex.Message}");
        }
    }
}
