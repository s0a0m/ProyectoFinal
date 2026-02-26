using System.Runtime.CompilerServices;
using src.Core.Services.Interfaces;
using src.Interfaces;
using src.Presentation.ViewModels.CuentaCorrienteVM;
using src.Repositories.Implementations;
using src.Repositories.Interfaces;
using Dom = src.Models.Domain;


namespace src.Core.Services.Implementations
{
    
    public class DocumentosAsociadoService: IDocumentoAsociadoService
    {
        private readonly IOrdenPagoRepository _ordenPagoRepository;
        private readonly IProveedorRepository  _proveedorRepository;

        private readonly IFacturaRepository _facturaRepository;

        public DocumentosAsociadoService(IOrdenPagoRepository ordenPagoRepository,IProveedorRepository  proveedorRepository,IFacturaRepository facturaRepository)
        {
            _ordenPagoRepository = ordenPagoRepository;
            _proveedorRepository = proveedorRepository;
            _facturaRepository = facturaRepository;
        }


        public async Task<OrdenPagoDetalleModalVM?> ObtenerDetalleParaModalAsync(int idOrden)
        {
            // Usamos el método que me pasaste del repositorio
            var orden = await _ordenPagoRepository.GetByIdWithDetallesAsync(idOrden);

            if (orden == null) return null;

            var vm = new OrdenPagoDetalleModalVM
            {
                IdOrdenPago = orden.IdOrdenPago,
                NumeroOrden = orden.Numero,
                FechaPago = orden.FechaPago ?? DateTime.MinValue,
                MontoTotal = orden.MontoTotal,
                Estado = orden.Enviada ? "Enviada" : "Pendiente",
                Items = orden.Detalles.Select(d => new DetallePagoItemVM
                {
                    // Asumo que tu entidad PagoDetalle tiene navegación a Factura
                    // Si no, avísame y ajustamos el repositorio para incluir Factura
                    NumeroFactura = d.Factura?.NumeroFactura ?? "S/N", 
                    FechaEmisionFactura = d.Factura?.FechaEmision ?? DateTime.MinValue,
                    ImportePagado = d.MontoAplicado// O el nombre del atributo importe en el detalle
                }).ToList()
            };

            return vm;
        }



        public async Task<ModuloOrdenesPagoVM> ObtenerModuloPorProveedorAsync(short idProveedor)
        {
            var proveedor = await _proveedorRepository.GetProveedorById(idProveedor);
            var ordenes = await _ordenPagoRepository.GetByProveedorAsync(idProveedor);

            return new ModuloOrdenesPagoVM
            {
                IdProveedor = idProveedor,
                RazonSocialProveedor = proveedor?.RazonSocial ?? "Desconocido",
                Ordenes = ordenes.Select(o => new OrdenPagoIndexVM
                {
                    IdOrdenPago = o.IdOrdenPago,
                    Numero = o.Numero,
                    FechaPago = o.FechaPago ?? DateTime.MinValue,
                    MontoTotal = o.MontoTotal,
                    Enviada = o.Enviada,
                    CantidadFacturasAfectadas = o.Detalles.Count
                }).ToList()
            };
        }

        // Prepara el VM con las facturas que tienen Saldo > 0
        public async Task<OrdenPagoFormVM> ObtenerFormularioCreacionAsync(short idProveedor)
        {
            var proveedor = await _proveedorRepository.GetProveedorById(idProveedor);
            // Usamos el repo de facturas para traer las del proveedor
            var facturas = await _facturaRepository.GetByProveedorAsync(idProveedor);

            // Filtramos solo las que tienen saldo pendiente
            var facturasImpagas = facturas.Where(f => f.Saldo > 0 && !f.Pagada).ToList();

            return new OrdenPagoFormVM
            {
                IdProveedor = idProveedor,
                RazonSocialProveedor = proveedor?.RazonSocial ?? string.Empty,
                FechaPago = DateTime.Now,
                FacturasDisponibles = facturasImpagas.Select(f => new SeleccionFacturaVM
                {
                    IdFactura = f.IdFactura,
                    NumeroFactura = f.NumeroFactura,
                    FechaEmision = f.FechaEmision,
                    TotalFacturado = f.TotalFacturado,
                    SaldoPendiente = f.Saldo,
                    MontoAPagar = f.Saldo, // Por defecto sugerimos pagar todo el saldo
                    EstaSeleccionada = false
                }).ToList()
            };
        }

        public async Task CrearOrdenAsync(OrdenPagoFormVM vm)
        {
            // 1. Crear la entidad dominio base
            var nuevaOrden = new Dom.OrdenPago
            {
                IdProveedor = (short)vm.IdProveedor,
                Numero = vm.NumeroOrden,
                FechaPago = null,
                Enviada = false, // Nace como borrador
                Detalles = new List<Dom.PagoDetalle>()
            };

            decimal sumaTotal = 0;

            // 2. Iterar sobre las facturas seleccionadas en el form
            foreach (var item in vm.FacturasDisponibles.Where(x => x.EstaSeleccionada))
            {
                if (item.MontoAPagar > 0)
                {
                    // Validacion simple: No pagar más que el saldo (opcional, depende de reglas de negocio)
                    // if(item.MontoAPagar > item.SaldoPendiente) throw new Exception(...);

                    var detalle = new Dom.PagoDetalle
                    {
                        IdFactura = item.IdFactura,
                        MontoAplicado = item.MontoAPagar
                    };
                    
                    nuevaOrden.Detalles.Add(detalle);
                    sumaTotal += item.MontoAPagar;
                }
            }

            nuevaOrden.MontoTotal = sumaTotal;

            // 3. Guardar (El repo ya hace el Add y SaveChanges)
            await _ordenPagoRepository.CreateAsync(nuevaOrden);
            
            // NOTA: Como nace "Enviada = false", NO actualizamos el saldo de las facturas todavía.
            // Eso se hará en el método "Enviar" o "Confirmar".
        }

        public async Task EliminarOrdenAsync(int idOrden)
        {
            var orden = await _ordenPagoRepository.GetByIdWithDetallesAsync(idOrden);
            if (orden == null) return;

            if (orden.Enviada)
            {
                throw new InvalidOperationException("No se puede eliminar una orden que ya fue enviada/procesada.");
            }

            await _ordenPagoRepository.DeleteAsync(idOrden);
        }



        public async Task ConfirmarOrdenAsync(int idOrden)
        {
            
            // 1. Obtener la orden con sus detalles
            // Usamos el método que ya creaste que trae los detalles
            var orden = await _ordenPagoRepository.GetByIdWithDetallesAsync(idOrden);
            ;
            if (orden == null) 
                throw new KeyNotFoundException("La orden de pago no existe.");
                
            if (orden.Enviada) 
                throw new InvalidOperationException("La orden ya fue enviada y procesada anteriormente.");

            var proveedor = await _proveedorRepository.GetProveedorById(orden.IdProveedor);

            if (proveedor == null)throw new Exception("Proveedor no encontrado");

            // 2. Iterar sobre los detalles para impactar en las facturas
            foreach (var detalle in orden.Detalles)
            {
                // Buscamos la factura fresca de la base de datos
                var factura = await _facturaRepository.GetByIdAsync(detalle.IdFactura);

                if (factura != null)
                {
                    // A. Descontamos el saldo
                    factura.Saldo -= detalle.MontoAplicado;

                    // B. Validación de seguridad para no tener saldos negativos (por decimales)
                    if (factura.Saldo < 0) 
                    {
                        factura.Saldo = 0; 
                    }

                    // C. Si el saldo llegó a 0, la marcamos como Pagada
                    // Usamos una pequeña tolerancia por si hay problemas de redondeo decimal
                    if (factura.Saldo <= 0.00m) 
                    {
                        factura.Pagada = true;
                        factura.Saldo = 0;
                        DateTime fec = DateTime.UtcNow;
                        factura.FechaPago = DateTime.SpecifyKind(fec, DateTimeKind.Utc);
                        
                    }

                    // D. Guardamos la actualización de la factura
                    await _facturaRepository.ActualizarSaldoYEstadoAsync(factura.IdFactura, factura.Saldo,factura.Pagada,factura.FechaPago);
                }
            }

            // 3. Cambiar el estado de la orden
            orden.Enviada = true;
            DateTime fe2 = DateTime.UtcNow;
            orden.FechaPago = DateTime.SpecifyKind(fe2, DateTimeKind.Utc);

            proveedor.Saldo -= orden.MontoTotal;
            if (proveedor.Saldo < 0)
                proveedor.Saldo = 0;
            

            // 4. Guardar la orden actualizada
            await _ordenPagoRepository.UpdateEstadoEnviadaAsync(idOrden, true, (DateTime)orden.FechaPago);
            await _proveedorRepository.ActualizarSaldoAsync(
                proveedor.IdProveedor,
                proveedor.Saldo
            );
        }




    }


}