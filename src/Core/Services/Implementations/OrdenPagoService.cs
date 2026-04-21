using Core.Common;
using src.Core.Contracts;
using src.Core.Services.Interfaces;
using src.Repositories.Interfaces;
using Dom = src.Models.Domain;

namespace src.Core.Services.Implementations;

public class OrdenPagoService : IOrdenPagoService
{
    private readonly IOrdenPagoRepository _ordenPagoRepository;
    private readonly IProveedorRepository _proveedorRepository;
    private readonly IFacturaRepository _facturaRepository;
    private readonly INumeracionService _numeracionService;
    private readonly IUnitOfWork _unitOfWork;

    public OrdenPagoService(
        IOrdenPagoRepository ordenPagoRepository,
        IProveedorRepository proveedorRepository,
        IFacturaRepository facturaRepository,
        INumeracionService numeracionService,
        IUnitOfWork unitOfWork
    )
    {
        _ordenPagoRepository = ordenPagoRepository;
        _proveedorRepository = proveedorRepository;
        _facturaRepository = facturaRepository;
        _numeracionService = numeracionService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<Dom.OrdenPago>> ObtenerDetalleOrdenAsync(int idOrden)
    {
        var orden = await _ordenPagoRepository.GetByIdWithDetallesAsync(idOrden);

        if (orden == null)
            return ServiceResult<Dom.OrdenPago>.Fail("La orden de pago solicitada no existe.");

        return ServiceResult<Dom.OrdenPago>.Ok(orden);
    }

    public async Task<ServiceResult<HistorialPagosData>> ObtenerHistorialPagosAsync(
        short idProveedor
    )
    {
        var proveedor = await _proveedorRepository.GetProveedorById(idProveedor);
        if (proveedor == null)
            return ServiceResult<HistorialPagosData>.Fail("Proveedor no encontrado.");

        var ordenes = await _ordenPagoRepository.GetByProveedorAsync(idProveedor);

        var data = new HistorialPagosData
        {
            IdProveedor = idProveedor,
            RazonSocial = proveedor.RazonSocial,
            Ordenes = ordenes.ToList(),
        };

        return ServiceResult<HistorialPagosData>.Ok(data);
    }

    public async Task<ServiceResult<NuevoPagoData>> ObtenerDatosParaNuevoPagoAsync(
        short idProveedor
    )
    {
        var proveedor = await _proveedorRepository.GetProveedorById(idProveedor);
        if (proveedor == null)
            return ServiceResult<NuevoPagoData>.Fail("El proveedor no existe.");

        var facturas = await _facturaRepository.GetByProveedorAsync(idProveedor);
        var pendientes = facturas.Where(f => f.Saldo > 0 && !f.Pagada).ToList();

        if (!pendientes.Any())
            return ServiceResult<NuevoPagoData>.Fail(
                "El proveedor no tiene facturas pendientes de pago."
            );

        return ServiceResult<NuevoPagoData>.Ok(
            new NuevoPagoData
            {
                IdProveedor = idProveedor,
                RazonSocial = proveedor.RazonSocial,
                FacturasPendientes = pendientes,
            }
        );
    }

    public async Task<ServiceResult<int>> CrearOrdenAsync(Dom.OrdenPago orden)
    {
        if (orden.Detalles == null || !orden.Detalles.Any())
            return ServiceResult<int>.Fail("Debe seleccionar al menos una factura.");

        decimal sumaCalculada = 0;

        foreach (var detalle in orden.Detalles)
        {
            var factura = await _facturaRepository.GetByIdAsync(detalle.IdFactura);
            if (factura == null)
                return ServiceResult<int>.Fail($"Factura ID {detalle.IdFactura} no encontrada.");

            if (!factura.PuedeRecibirPago)
                return ServiceResult<int>.Fail($"La factura {factura.Numero} ya está pagada.");

            if (detalle.MontoAplicado <= 0)
                return ServiceResult<int>.Fail(
                    $"El monto para la factura {factura.Numero} debe ser mayor a cero."
                );

            if (detalle.MontoAplicado > factura.Saldo)
                return ServiceResult<int>.Fail(
                    $"El monto aplicado a {factura.Numero} supera el saldo pendiente."
                );

            sumaCalculada += detalle.MontoAplicado;
        }

        orden.MontoTotal = sumaCalculada;
        orden.Numero = await _numeracionService.GenerarNumeroAsync("OP-", "OP");
        orden.Enviada = false;
        orden.FechaPago = null;

        try
        {
            await _ordenPagoRepository.CreateAsync(orden);
            return ServiceResult<int>.Ok(orden.IdOrdenPago, "Borrador de orden de pago creado.");
        }
        catch (Exception ex)
        {
            return ServiceResult<int>.Fail($"Error en base de datos: {ex.Message}");
        }
    }

    public async Task<ServiceResult> ConfirmarOrdenAsync(int idOrden)
    {
        var orden = await _ordenPagoRepository.GetByIdWithDetallesAsync(idOrden);
        if (orden == null)
            return ServiceResult.Fail("La orden de pago no existe.");
        if (orden.Enviada)
            return ServiceResult.Fail(
                "Esta orden ya fue confirmada y no puede procesarse nuevamente."
            );

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            foreach (var detalle in orden.Detalles)
            {
                var factura = await _facturaRepository.GetByIdAsync(detalle.IdFactura);
                if (factura == null)
                    continue;
                if (!factura.PuedeRecibirPago || detalle.MontoAplicado > factura.Saldo)
                {
                    await _unitOfWork.RollbackAsync();
                    return ServiceResult.Fail(
                        $"El saldo de la factura {factura.Numero} cambió. Elimine este borrador y cree uno nuevo."
                    );
                }

                factura.AplicarPago(detalle.MontoAplicado);
                await _facturaRepository.ActualizarSaldoYEstadoAsync(
                    factura.IdFactura,
                    factura.Saldo,
                    factura.Pagada,
                    factura.FechaPago
                );
            }

            orden.Enviada = true;
            orden.FechaPago = DateTime.UtcNow;
            await _ordenPagoRepository.UpdateEstadoEnviadaAsync(
                idOrden,
                true,
                orden.FechaPago.Value
            );

            var proveedor = await _proveedorRepository.GetProveedorById(orden.IdProveedor);
            if (proveedor != null)
            {
                proveedor.SaldoActual -= orden.MontoTotal;
                await _proveedorRepository.ActualizarSaldoActualAsync(
                    proveedor.IdProveedor,
                    proveedor.SaldoActual
                );
            }

            await _unitOfWork.CommitAsync();
            return ServiceResult.Ok(
                "Pago confirmado exitosamente. Los saldos han sido actualizados."
            );
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return ServiceResult.Fail($"Error crítico en confirmación: {ex.Message}");
        }
    }

    public async Task<ServiceResult> EliminarOrdenAsync(int idOrden)
    {
        var orden = await _ordenPagoRepository.GetByIdWithDetallesAsync(idOrden);

        if (orden == null)
            return ServiceResult.Fail("La orden que intenta eliminar no existe.");

        if (orden.Enviada)
            return ServiceResult.Fail(
                "No es posible eliminar una orden confirmada. Debe proceder a una anulación si el sistema lo permite."
            );

        try
        {
            await _ordenPagoRepository.DeleteAsync(idOrden);
            return ServiceResult.Ok("El borrador de la orden fue eliminado.");
        }
        catch (Exception ex)
        {
            return ServiceResult.Fail($"No se pudo eliminar el borrador: {ex.Message}");
        }
    }
}
