using System.Runtime.CompilerServices;
using src.Core.Services.Interfaces;
using src.Interfaces;
using src.Models.Common;
using src.Models.Domain;
using src.Models.Mappers;
using src.Presentation.ViewModels.CompraVM;
using src.Presentation.ViewModels.FacturaVM;
using src.Repositories.Implementations;
using src.Repositories.Interfaces;
using Dom = src.Models.Domain;

namespace src.Core.Services.Implementations
{
    

        // FacturaService.cs
public class FacturaService : IFacturaService
{
    private readonly IFacturaRepository _facturaRepository;
    private readonly ICondicionPagoRepository _condicionPagoRepository;
    private readonly ICompraRepository _compraRepository;
    private readonly IComprobanteRepository _comprobanteRepository;
    private readonly IOrdenPagoRepository _ordenPagoRepository;
    
    // Actualiza el constructor
    public FacturaService(
        IFacturaRepository facturaRepo, 
        ICondicionPagoRepository condicionPagoRepository,
        ICompraRepository compraRepository,IComprobanteRepository comprobanteRepository,
        IOrdenPagoRepository ordenPagoRepository) 
    {
        _facturaRepository = facturaRepo;
        _condicionPagoRepository = condicionPagoRepository;
        _compraRepository = compraRepository;
        _comprobanteRepository = comprobanteRepository;
        _ordenPagoRepository = ordenPagoRepository;
    }

    public Task<bool> ActualizarEstadoPagoAsync(int id, bool pagada)
        {
            throw new NotImplementedException();
        }

    public async Task<CrearFacturaViewModel> PrepararFacturaDesdeCompraAsync(int idCompra)
    {
        var compra = await _compraRepository.GetByIdAsync(idCompra);
        
        if (compra == null) throw new KeyNotFoundException($"No se encontró la compra");
        if (compra.Estado.ToString() != "ENVIADA") throw new InvalidOperationException("Solo se pueden facturar compras ENVIADA.");
            
        var vm = new CrearFacturaViewModel
        {
            IdCompra = compra.IdCompra,
            IdProveedor = compra.Proveedor.IdProveedor,
            RazonSocial = compra.Proveedor?.RazonSocial ?? "Desconocido",
            FechaEmision = DateTime.Now,
            TipoCondicion = "Contado",
            DiasPago = 0
        };

        if (compra.Proveedor?.Condicion != null)
        {
            vm.DiasPago = compra.Proveedor.Condicion.DiasPago;
            
            if (compra.Proveedor.Condicion is Dom.Cuota c)
            {
                vm.TipoCondicion = "Cuota";
                vm.CantidadCuotas = c.Cuotas;
                vm.InteresPorcentual = c.InteresPorcentual;
            }
            else
            {
                vm.TipoCondicion = "Contado";
            }
        }

        vm.Detalles = compra.Detalles.Select(d => new CrearFacturaDetalleViewModel
        {
            IdProducto = d.Producto.IdProducto,
            NombreProducto = d.Producto?.Nombre ?? "Producto",
            Cantidad = d.Cantidad,
            PrecioBruto = d.PrecioPactado,
            PorcentajeDescuento = 0,
            PrecioNeto = d.PrecioPactado, // Inicialmente igual al pactado
            // Subtotal se calcula en el getter o en la vista
        }).ToList();
        return vm;
    }

    
    public async Task<int> CrearFacturaAsync(CrearFacturaViewModel modelo)
    {
    Dom.CondicionDePago condicionResolver;

    if (modelo.TipoCondicion == "Cuota")
    {
        condicionResolver = new Dom.Cuota
        {
            DiasPago = modelo.DiasPago,
            Cuotas = modelo.CantidadCuotas ?? 1,
            InteresPorcentual = modelo.InteresPorcentual ?? 0
        };
    }
    else
    {
        condicionResolver = new Dom.Contado
        {
            DiasPago = modelo.DiasPago
        };
    }

    var condicionFinal = await _condicionPagoRepository.BuscarOCrearAsync(condicionResolver);
    var nuevaFactura = new Dom.Factura
    {
        Compra = new Dom.Compra { IdCompra = modelo.IdCompra },
        Proveedor = new Dom.Proveedor { IdProveedor = (short)modelo.IdProveedor },
        NumeroFactura = modelo.NumeroFactura,
       FechaEmision = DateTime.SpecifyKind(modelo.FechaEmision, DateTimeKind.Utc),
        CondicionPago = condicionFinal,
        Pagada = false,
        Detalles = new List<Dom.DetalleFactura>()
    };

    decimal acumuladorTotal = 0;

    foreach (var item in modelo.Detalles)
    {
        // Validar integridad básica (precios no negativos, etc)
        var detalle = new Dom.DetalleFactura
        {
            Producto = new Dom.Producto { IdProducto = (short)item.IdProducto },
            Cantidad = item.Cantidad,
            PrecioBruto = item.PrecioBruto,
            PorcentajeDescuento = item.PorcentajeDescuento,
            PrecioNeto = item.PrecioNeto 
        };
        // Calcular subtotal real basado en precio neto final
        decimal subtotalItem = item.Cantidad * item.PrecioNeto;
        acumuladorTotal += subtotalItem;
        
        nuevaFactura.Detalles.Add(detalle);
    }

        nuevaFactura.TotalFacturado = acumuladorTotal;
        nuevaFactura.Saldo = acumuladorTotal;

        var facturaCreada = await _facturaRepository.AddAsync(nuevaFactura);
        await _compraRepository.CompletarCompraAsync(modelo.IdCompra);
        return facturaCreada.IdFactura;
    }


        public async Task<bool> ExisteNumeroFacturaAsync(short idProveedor, string numero)
        {
            return await _facturaRepository.ExisteNumeroFacturaAsync(idProveedor, numero);
        }

        public async Task<IEnumerable<ListarFacturaViewModel>> ObtenerPendientesPagoAsync()
        {
            var facturas = await _facturaRepository.GetPendientesPagoAsync();

            return ListarFacturaViewModel.MapFacturaVM(facturas);
        }
        public async Task<ListarDetalleFacturaViewModel?> ObtenerPorIdAsync(int id)
        {
            var factura = await _facturaRepository.GetByIdAsync(id);
            if (factura == null) return null;
            return ListarDetalleFacturaViewModel.MapListarDetalleVM(factura);
        }

        public async Task<IEnumerable<ListarFacturaViewModel>> ObtenerTodasAsync()
        {
            var facturas = await _facturaRepository.GetAllAsync();

            return ListarFacturaViewModel.MapFacturaVM(facturas);
        }

        // En src/Core/Services/Implementations/FacturaService.cs

        public async Task<ActualizarFacturaViewModel> PrepararEdicionFacturaAsync(int idFactura)
        {
            var factura = await _facturaRepository.GetByIdAsync(idFactura);
            if (factura == null) throw new KeyNotFoundException("Factura no encontrada");

            // Mapeo inverso de Dominio a ViewModel
            var vm = new ActualizarFacturaViewModel
            {
                IdFactura = factura.IdFactura,
                IdCompra = factura.Compra?.IdCompra ?? 0,
                IdProveedor = factura.Proveedor?.IdProveedor ?? 0,
                RazonSocial = factura.Proveedor?.RazonSocial ?? "Desconocido",
                NumeroFactura = factura.NumeroFactura,
                FechaEmision = factura.FechaEmision,
                
                // Mapeo de Condición de Pago
                TipoCondicion = "Contado",
                DiasPago = 0
            };

            if (factura.CondicionPago != null)
            {
                vm.DiasPago = factura.CondicionPago.DiasPago;
                if (factura.CondicionPago is Dom.Cuota c)
                {
                    vm.TipoCondicion = "Cuota";
                    vm.CantidadCuotas = c.Cuotas;
                    vm.InteresPorcentual = c.InteresPorcentual;
                }
            }

            // Mapeo de Detalles
            vm.Detalles = factura.Detalles.Select(d => new CrearFacturaDetalleViewModel
            {
                IdProducto = d.Producto.IdProducto,
                NombreProducto = d.Producto?.Nombre ?? "Producto",
                Cantidad = d.Cantidad,
                PrecioBruto = d.PrecioBruto,
                PorcentajeDescuento = d.PorcentajeDescuento,
                PrecioNeto = d.PrecioNeto,
                // Subtotal es calculado
            }).ToList();

            return vm;
        }

        public async Task UpdateAsync(int id, ActualizarFacturaViewModel model)
        {
            // 1. Resolver Condición de Pago (Igual que en crear)
            Dom.CondicionDePago condicionResolver;
            if (model.TipoCondicion == "Cuota")
            {
                condicionResolver = new Dom.Cuota { 
                    DiasPago = model.DiasPago, 
                    Cuotas = model.CantidadCuotas ?? 1, 
                    InteresPorcentual = model.InteresPorcentual ?? 0 
                };
            }
            else
            {
                condicionResolver = new Dom.Contado { DiasPago = model.DiasPago };
            }
            
            var condicionFinal = await _condicionPagoRepository.BuscarOCrearAsync(condicionResolver);

            // 2. Armar objeto Dominio
            var facturaDom = new Dom.Factura
            {
                IdFactura = id,
                NumeroFactura = model.NumeroFactura,
                // FIX DE FECHA UTC IMPORTANTE
                FechaEmision = DateTime.SpecifyKind(model.FechaEmision, DateTimeKind.Utc),
                CondicionPago = condicionFinal,
                Detalles = new List<Dom.DetalleFactura>()
            };

            decimal total = 0;
            foreach(var item in model.Detalles)
            {
                var det = new Dom.DetalleFactura
                {
                    Producto = new Dom.Producto { IdProducto = (short)item.IdProducto },
                    Cantidad = item.Cantidad,
                    PrecioBruto = item.PrecioBruto,
                    PorcentajeDescuento = item.PorcentajeDescuento,
                    PrecioNeto = item.PrecioNeto
                };
                total += item.Cantidad * item.PrecioNeto;
                facturaDom.Detalles.Add(det);
            }
            facturaDom.TotalFacturado = total;

            // 3. Llamar al repo
            await _facturaRepository.UpdateAsync(facturaDom);
        }



        public async Task<DocumentosRelacionadosViewModel> ObtenerDocumentosAsociadosAsync(int idFactura)
        {
            var factura = await _facturaRepository.GetByIdAsync(idFactura);
            if (factura == null) throw new KeyNotFoundException($"No se encontró la factura #{idFactura}");

            var comprobantesDom = await _comprobanteRepository.GetByFacturaIdAsync(idFactura);
            var ordenesDom = await _ordenPagoRepository.GetPagosPorFacturaIdAsync(idFactura);

            var vm = new DocumentosRelacionadosViewModel
            {
                IdFactura = factura.IdFactura,
                NumeroFactura = factura.NumeroFactura,
                Proveedor = factura.Proveedor?.RazonSocial ?? "Desconocido",
                
                Comprobantes = comprobantesDom.Select(c => new ResumenComprobanteViewModel
                {
                    IdComprobante = c.IdComprobante,
                    Numero = c.Numero,
                    FechaEmision = c.FechaEmision,
                    Total = c.Total,
                    Comentario = c.Comentario,
                    Motivo = c.Motivo?.Descripcion ?? "Sin motivo",
                    Tipo = c is Dom.NotaCredito ? "Nota de Crédito" : c is Dom.NotaDebito ? "Nota de Débito" : "Comprobante"
                }).ToList(),

                // --- MAPEO ACTUALIZADO CON TUS NUEVOS ATRIBUTOS ---
                OrdenesPago = ordenesDom.Select(op => new ResumenOrdenPagoViewModel
                {
                    IdOrdenPago = op.IdOrdenPago,
                    Numero = op.Numero,
                    FechaPago = op.FechaPago,
                    TotalOrden = op.MontoTotal,
                    Enviada = op.Enviada,
                    
                    // Calculamos cuánto de esta OP corresponde a ESTA factura
                    MontoAplicado = op.Detalles
                        .Where(d => d.IdFactura == idFactura)
                        .Sum(d => d.MontoAplicado) 
                }).ToList()
            };

            return vm;
        }





    }
}