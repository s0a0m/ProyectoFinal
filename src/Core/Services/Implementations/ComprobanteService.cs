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

        public ComprobanteService(
            IComprobanteRepository comprobanteRepository,
            IFacturaRepository facturaRepository,
            IProveedorRepository proveedorRepository,
            INumeracionService numeracionService
        )
        {
            _comprobanteRepository = comprobanteRepository;
            _facturaRepository = facturaRepository;
            _proveedorRepository = proveedorRepository;
            _numeracionService = numeracionService;
        }

        public async Task<IEnumerable<Comprobante>> ObtenerPorFacturaAsync(int facturaId)
        {
            return await _comprobanteRepository.GetByFacturaIdAsync(facturaId);
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
                return ServiceResult<int>.Fail("La factura no existe");

            if (factura.Proveedor == null)
                return ServiceResult<int>.Fail("La factura no tiene un proveedor asociado");

            if (comprobante.Total <= 0)
                return ServiceResult<int>.Fail("El total debe ser mayor a cero");

            if (comprobante is Dom.NotaCredito)
            {
                if (!factura.PuedeEmitirNC)
                    return ServiceResult<int>.Fail(
                        "No se puede emitir NC sobre una factura pagada"
                    );
                if (comprobante.Total > factura.Saldo)
                    return ServiceResult<int>.Fail("La NC no puede superar el saldo pendiente");
            }

            if (comprobante is Dom.NotaDebito && !factura.PuedeEmitirND)
                return ServiceResult<int>.Fail("No se puede emitir ND sobre esta factura");

            comprobante.Proveedor = factura.Proveedor;
            comprobante.FechaEmision = DateTime.UtcNow;
            comprobante.FacturaOriginal = factura;
            var prefijo = comprobante is Dom.NotaCredito ? "NC-" : "ND-";
            comprobante.Numero = await _numeracionService.GenerarNumeroAsync(
                prefijo,
                prefijo.Replace("-", "")
            );
            try
            {
                comprobante.Aplicar(factura, factura.Proveedor);
                var comprobanteCreado = await _comprobanteRepository.AddAsync(comprobante);
                await _facturaRepository.UpdateAsync(factura);
                await _proveedorRepository.ActualizarSaldoAsync(
                    factura.Proveedor.IdProveedor,
                    factura.Proveedor.Saldo
                );

                return ServiceResult<int>.Ok(
                    comprobanteCreado.IdComprobante,
                    "Comprobante creado correctamente."
                );
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail($"Error al guardar: {ex.Message}");
            }
        }

        public async Task<GestionNotasData?> ObtenerGestionNotasAsync(int idProveedor)
        {
            var proveedor = await _proveedorRepository.GetProveedorById(idProveedor);
            if (proveedor == null)
                return null;

            var facturas = await _facturaRepository.GetByProveedorAsync((short)idProveedor);
            var motivosNC = await _comprobanteRepository.GetMotivosAsync("NC");
            var motivosND = await _comprobanteRepository.GetMotivosAsync("ND");

            var result = new GestionNotasData
            {
                Proveedor = proveedor,
                MotivosNC = motivosNC.ToList(),
                MotivosND = motivosND.ToList(),
            };

            foreach (var factura in facturas)
            {
                var comprobantes = await _comprobanteRepository.GetByFacturaIdAsync(
                    factura.IdFactura
                );
                result.Facturas.Add(
                    new FacturaConNotas { Factura = factura, Comprobantes = comprobantes.ToList() }
                );
            }

            return result;
        }
    }
}
