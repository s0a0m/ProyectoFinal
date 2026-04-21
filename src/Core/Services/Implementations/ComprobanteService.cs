using Core.Common;
using src.Core.Contracts;
using src.Core.Services.Interfaces;
using src.Models.Domain;
using src.Repositories.Interfaces;
using Dom = src.Models.Domain;

namespace src.Core.Services.Implementations
{
    public class ComprobanteService : IComprobanteService
    {
        private readonly IComprobanteRepository _comprobanteRepository;
        private readonly IFacturaRepository _facturaRepository;
        private readonly IProveedorRepository _proveedorRepository;
        private readonly INumeracionService _numeracionService;
        private readonly IUnitOfWork _unitOfWork;

        public ComprobanteService(
            IComprobanteRepository comprobanteRepository,
            IFacturaRepository facturaRepository,
            IProveedorRepository proveedorRepository,
            INumeracionService numeracionService,
            IUnitOfWork unitOfWork
        )
        {
            _comprobanteRepository = comprobanteRepository;
            _facturaRepository = facturaRepository;
            _proveedorRepository = proveedorRepository;
            _numeracionService = numeracionService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<IEnumerable<Dom.Comprobante>>> ObtenerPorFacturaAsync(
            int facturaId
        )
        {
            var comprobantes = await _comprobanteRepository.GetByFacturaIdAsync(facturaId);
            return ServiceResult<IEnumerable<Dom.Comprobante>>.Ok(comprobantes);
        }

        public async Task<ServiceResult<Dom.Comprobante>> ObtenerDetalleNotaAsync(int id)
        {
            var comprobante = await _comprobanteRepository.GetByIdAsync(id);

            if (comprobante == null)
                return ServiceResult<Dom.Comprobante>.Fail("El comprobante solicitado no existe.");

            return ServiceResult<Dom.Comprobante>.Ok(comprobante);
        }

        public async Task<Comprobante?> ObtenerPorIdAsync(int id)
        {
            return await _comprobanteRepository.GetByIdAsync(id);
        }

        public async Task<ServiceResult<int>> CrearComprobanteAsync(Dom.Comprobante comprobante)
        {
            var factura = await _facturaRepository.GetByIdAsync(
                comprobante.IdFacturaReferencia!.Value
            );

            if (factura == null)
                return ServiceResult<int>.Fail("La factura de referencia no existe.");

            if (comprobante is Dom.NotaDebito && factura.Pagada)
            {
                return ServiceResult<int>.Fail(
                    "No se puede emitir una Nota de Débito sobre una factura que ya ha sido totalmente cancelada."
                );
            }

            if (comprobante.Total <= 0)
                return ServiceResult<int>.Fail("El monto debe ser mayor a cero.");

            if (comprobante is Dom.NotaCredito)
            {
                if (factura.Pagada)
                    return ServiceResult<int>.Fail(
                        "No se puede emitir una NC sobre una factura ya cancelada."
                    );
                if (comprobante.Total > factura.Saldo)
                    return ServiceResult<int>.Fail(
                        "El monto de la NC no puede ser mayor al saldo pendiente."
                    );
            }

            comprobante.Proveedor = factura.Proveedor;
            comprobante.FechaEmision = DateTime.UtcNow;
            comprobante.FacturaOriginal = factura;

            string prefijo = comprobante is Dom.NotaCredito ? "NC-" : "ND-";
            comprobante.Numero = await _numeracionService.GenerarNumeroAsync(
                prefijo,
                prefijo.Replace("-", "")
            );

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                comprobante.Aplicar(factura, factura.Proveedor);

                var nuevoComprobante = await _comprobanteRepository.AddAsync(comprobante);

                await _facturaRepository.UpdateAsync(factura);

                await _proveedorRepository.ActualizarSaldoActualAsync(
                    factura.Proveedor.IdProveedor,
                    factura.Proveedor.SaldoActual
                );

                await _unitOfWork.CommitAsync();
                return ServiceResult<int>.Ok(
                    nuevoComprobante.IdComprobante,
                    "Comprobante procesado y saldos actualizados."
                );
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return ServiceResult<int>.Fail($"Error al procesar el comprobante: {ex.Message}");
            }
        }

        public async Task<ServiceResult<GestionNotasData>> ObtenerGestionNotasAsync(int idProveedor)
        {
            var proveedor = await _proveedorRepository.GetProveedorById((short)idProveedor);
            if (proveedor == null)
                return ServiceResult<GestionNotasData>.Fail("Proveedor no encontrado.");

            var facturas = await _facturaRepository.GetByProveedorAsync((short)idProveedor);
            var comprobantes = await _comprobanteRepository.GetByProveedorAsync((short)idProveedor);

            var motivosNC = await _comprobanteRepository.GetMotivosAsync("NC");
            var motivosND = await _comprobanteRepository.GetMotivosAsync("ND");

            var result = new GestionNotasData
            {
                Proveedor = proveedor,
                MotivosNC = motivosNC.ToList(),
                MotivosND = motivosND.ToList(),
                Facturas = facturas
                    .Select(f => new FacturaConNotas
                    {
                        Factura = f,
                        Comprobantes = comprobantes
                            .Where(c => c.IdFacturaReferencia == f.IdFactura)
                            .ToList(),
                    })
                    .ToList(),
            };

            return ServiceResult<GestionNotasData>.Ok(result);
        }
    }
}
