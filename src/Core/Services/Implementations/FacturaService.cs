using Core.Common;
using src.Core.Contracts;
using src.Core.Services.Interfaces;
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

    public FacturaService(
        IFacturaRepository facturaRepo,
        ICondicionPagoRepository condicionPagoRepository,
        ICompraRepository compraRepository,
        IComprobanteRepository comprobanteRepository,
        IOrdenPagoRepository ordenPagoRepository
    )
    {
        _facturaRepository = facturaRepo;
        _condicionPagoRepository = condicionPagoRepository;
        _compraRepository = compraRepository;
        _comprobanteRepository = comprobanteRepository;
        _ordenPagoRepository = ordenPagoRepository;
    }

    public async Task<IEnumerable<Dom.Factura>> ObtenerTodasAsync()
    {
        return await _facturaRepository.GetAllAsync();
    }

    public async Task<Dom.Factura?> ObtenerPorIdAsync(int id)
    {
        return await _facturaRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Dom.Factura>> ObtenerPendientesPagoAsync()
    {
        return await _facturaRepository.GetPendientesPagoAsync();
    }

    public async Task<bool> ExisteNumeroFacturaAsync(short idProveedor, string numero)
    {
        return await _facturaRepository.ExisteNumeroFacturaAsync(idProveedor, numero);
    }

    public async Task<DocumentosRelacionadosData?> ObtenerDocumentosAsociadosAsync(int idFactura)
    {
        var factura = await _facturaRepository.GetByIdAsync(idFactura);
        if (factura == null)
            return null;

        var comprobantes = await _comprobanteRepository.GetByFacturaIdAsync(idFactura);
        var ordenes = await _ordenPagoRepository.GetPagosPorFacturaIdAsync(idFactura);

        return new DocumentosRelacionadosData
        {
            Factura = factura,
            Comprobantes = comprobantes.ToList(),
            OrdenesPago = ordenes.ToList(),
        };
    }

    public async Task<ServiceResult<int>> CrearDesdeCompraAsync(Dom.Factura factura, int idCompra)
    {
        var compra = await _compraRepository.GetByIdAsync(idCompra);
        if (compra == null)
            return ServiceResult<int>.Fail("Compra no encontrada");

        if (compra.Estado.ToString() != "ENVIADA")
            return ServiceResult<int>.Fail("Solo se pueden facturar compras en estado ENVIADA");

        // Resolver condición de pago
        if (factura.CondicionPago != null)
        {
            factura.CondicionPago = await _condicionPagoRepository.BuscarOCrearAsync(
                factura.CondicionPago
            );
        }

        // Calcular totales
        decimal total = factura.Detalles.Sum(d => d.Cantidad * d.PrecioNeto);
        factura.TotalFacturado = total;
        factura.Saldo = total;
        factura.Pagada = false;

        try
        {
            var facturaCreada = await _facturaRepository.AddAsync(factura);
            await _compraRepository.CompletarCompraAsync(idCompra);
            return ServiceResult<int>.Ok(facturaCreada.IdFactura, "Factura creada correctamente");
        }
        catch (Exception ex)
        {
            return ServiceResult<int>.Fail($"Error al crear la factura: {ex.Message}");
        }
    }

    public async Task<ServiceResult> ActualizarAsync(Dom.Factura factura)
    {
        var facturaVieja = await _facturaRepository.GetByIdAsync(factura.IdFactura);
        if (facturaVieja == null)
            return ServiceResult.Fail("Factura no encontrada");

        // Resolver condición de pago
        if (factura.CondicionPago != null)
        {
            factura.CondicionPago = await _condicionPagoRepository.BuscarOCrearAsync(
                factura.CondicionPago
            );
        }

        // Recalcular saldo respetando pagos previos
        decimal total = factura.Detalles.Sum(d => d.Cantidad * d.PrecioNeto);
        decimal montoPagado = facturaVieja.TotalFacturado - facturaVieja.Saldo;

        factura.TotalFacturado = total;
        factura.Saldo = Math.Max(0, total - montoPagado);
        factura.Pagada = factura.Saldo == 0;

        try
        {
            await _facturaRepository.UpdateAsync(factura);
            return ServiceResult.Ok("Factura actualizada correctamente");
        }
        catch (Exception ex)
        {
            return ServiceResult.Fail($"Error al actualizar: {ex.Message}");
        }
    }
}
