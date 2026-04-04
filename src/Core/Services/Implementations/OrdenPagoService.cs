using Core.Common;
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

    public OrdenPagoService(
        IOrdenPagoRepository ordenPagoRepository,
        IProveedorRepository proveedorRepository,
        IFacturaRepository facturaRepository,
        INumeracionService numeracionService
    )
    {
        _ordenPagoRepository = ordenPagoRepository;
        _proveedorRepository = proveedorRepository;
        _facturaRepository = facturaRepository;
        _numeracionService = numeracionService;
    }

    public async Task<Dom.OrdenPago?> ObtenerPorIdAsync(int idOrden)
    {
        return await _ordenPagoRepository.GetByIdWithDetallesAsync(idOrden);
    }

    public async Task<IEnumerable<Dom.OrdenPago>> ObtenerPorProveedorAsync(short idProveedor)
    {
        return await _ordenPagoRepository.GetByProveedorAsync(idProveedor);
    }

    public async Task<IEnumerable<Dom.Factura>> ObtenerFacturasPendientesAsync(short idProveedor)
    {
        var facturas = await _facturaRepository.GetByProveedorAsync(idProveedor);
        return facturas.Where(f => f.Saldo > 0 && !f.Pagada);
    }

    public async Task<ServiceResult<int>> CrearOrdenAsync(Dom.OrdenPago orden)
    {
        if (orden.Detalles == null || !orden.Detalles.Any())
            return ServiceResult<int>.Fail("Debe seleccionar al menos una factura");

        if (orden.MontoTotal <= 0)
            return ServiceResult<int>.Fail("El monto total debe ser mayor a cero");

        // Validar cada factura
        foreach (var detalle in orden.Detalles)
        {
            var factura = await _facturaRepository.GetByIdAsync(detalle.IdFactura);
            if (factura == null)
                return ServiceResult<int>.Fail($"Factura {detalle.IdFactura} no encontrada");

            if (!factura.PuedeRecibirPago)
                return ServiceResult<int>.Fail(
                    $"La factura {factura.Numero} no tiene saldo pendiente"
                );

            if (detalle.MontoAplicado > factura.Saldo)
                return ServiceResult<int>.Fail(
                    $"El monto {detalle.MontoAplicado} supera el saldo "
                        + $"({factura.Saldo}) de la factura {factura.Numero}"
                );
        }
        orden.Numero = await _numeracionService.GenerarNumeroAsync("OP-", "OP");
        orden.Enviada = false;
        orden.FechaPago = null;

        try
        {
            await _ordenPagoRepository.CreateAsync(orden);
            return ServiceResult<int>.Ok(orden.IdOrdenPago, "Orden creada correctamente");
        }
        catch (Exception ex)
        {
            return ServiceResult<int>.Fail($"Error al crear la orden: {ex.Message}");
        }
    }

    public async Task<ServiceResult> ConfirmarOrdenAsync(int idOrden)
    {
        var orden = await _ordenPagoRepository.GetByIdWithDetallesAsync(idOrden);
        if (orden == null)
            return ServiceResult.Fail("La orden de pago no existe");

        if (orden.Enviada)
            return ServiceResult.Fail("La orden ya fue confirmada anteriormente");

        var proveedor = await _proveedorRepository.GetProveedorById(orden.IdProveedor);
        if (proveedor == null)
            return ServiceResult.Fail("Proveedor no encontrado");

        try
        {
            foreach (var detalle in orden.Detalles)
            {
                var factura = await _facturaRepository.GetByIdAsync(detalle.IdFactura);
                if (factura == null)
                    continue;

                // Validar saldo actual (pudo cambiar por NC desde que se creó el borrador)
                if (!factura.PuedeRecibirPago)
                    return ServiceResult.Fail(
                        $"La factura {factura.Numero} ya no tiene saldo pendiente. "
                            + $"Elimine esta orden y cree una nueva."
                    );

                if (detalle.MontoAplicado > factura.Saldo)
                    return ServiceResult.Fail(
                        $"El monto {detalle.MontoAplicado} supera el saldo actual "
                            + $"({factura.Saldo}) de la factura {factura.Numero}. "
                            + $"Elimine esta orden y cree una nueva."
                    );

                factura.AplicarPago(detalle.MontoAplicado);

                await _facturaRepository.ActualizarSaldoYEstadoAsync(
                    factura.IdFactura,
                    factura.Saldo,
                    factura.Pagada,
                    factura.FechaPago
                );
            }

            proveedor.ReducirSaldo(orden.MontoTotal);

            orden.Enviada = true;
            orden.FechaPago = DateTime.UtcNow;

            await _ordenPagoRepository.UpdateEstadoEnviadaAsync(
                idOrden,
                true,
                orden.FechaPago.Value
            );
            await _proveedorRepository.ActualizarSaldoActualAsync(
                proveedor.IdProveedor,
                proveedor.SaldoInicial
            );

            return ServiceResult.Ok("Orden confirmada correctamente");
        }
        catch (Exception ex)
        {
            return ServiceResult.Fail($"Error al confirmar: {ex.Message}");
        }
    }

    public async Task<ServiceResult> EliminarOrdenAsync(int idOrden)
    {
        var orden = await _ordenPagoRepository.GetByIdWithDetallesAsync(idOrden);
        if (orden == null)
            return ServiceResult.Fail("La orden no existe");

        if (orden.Enviada)
            return ServiceResult.Fail("No se puede eliminar una orden ya confirmada");

        try
        {
            await _ordenPagoRepository.DeleteAsync(idOrden);
            return ServiceResult.Ok("Orden eliminada correctamente");
        }
        catch (Exception ex)
        {
            return ServiceResult.Fail($"Error al eliminar: {ex.Message}");
        }
    }
}
