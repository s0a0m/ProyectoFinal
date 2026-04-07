using src.Core.Contracts;
using src.Presentation.ViewModels.FacturaVM;
using Dom = src.Models.Domain;

namespace src.Presentation.Mappers;

public static class FacturaMapper
{
    // Domain → VM (listado)
    public static ListarFacturaViewModel ToListVM(this Dom.Factura factura)
    {
        return new ListarFacturaViewModel
        {
            IdFactura = factura.IdFactura,
            NumeroComprobante = factura.Numero,
            Proveedor = factura.Proveedor?.RazonSocial ?? "Desconocido",
            Fecha = factura.FechaEmision,
            FechaPago = factura.FechaPago,
            Total = factura.TotalFacturado,
            Saldo = factura.Saldo,
            EstadoPago = factura.Pagada ? "Pagada" : "Pendiente",
            CantidadItems = factura.Detalles?.Count ?? 0,
        };
    }

    public static IEnumerable<ListarFacturaViewModel> ToListVM(
        this IEnumerable<Dom.Factura> facturas
    )
    {
        return facturas.Select(f => f.ToListVM());
    }

    // Domain → VM (detalle)
    public static ListarDetalleFacturaViewModel ToDetalleVM(this Dom.Factura factura)
    {
        return new ListarDetalleFacturaViewModel
        {
            IdFactura = factura.IdFactura,
            NumeroComprobante = factura.Numero,
            Fecha = factura.FechaEmision,
            FechaPago = factura.FechaPago,
            Total = factura.TotalFacturado,
            Saldo = factura.Saldo,
            EstadoPago = factura.Pagada ? "Pagada" : "Pendiente",
            ProveedorNombre = factura.Proveedor?.RazonSocial ?? "Desconocido",
            CuitProveedor = factura.Proveedor?.Cuit ?? "N/A",
            CondicionPagoDesc = factura.CondicionPago switch
            {
                Dom.Cuota c =>
                    $"{c.Cuotas} cuotas (Interés: {c.InteresPorcentual}%) - Vence a los {c.DiasPago} días",
                Dom.Contado => $"Contado - Pago a los {factura.CondicionPago.DiasPago} días",
                _ => factura.CondicionPago != null
                    ? $"Plazo: {factura.CondicionPago.DiasPago} días"
                    : "No especificada",
            },
            Items = factura
                .Detalles.Select(d => new ItemFacturaViewModel
                {
                    ProductoNombre = d.Producto?.Nombre ?? "Producto no identificado",
                    Cantidad = d.Cantidad,
                    PrecioBruto = d.PrecioBruto,
                    PorcentajeDescuento = d.PorcentajeDescuento,
                    PrecioNeto = d.PrecioNeto,
                    Subtotal = d.Subtotal,
                })
                .ToList(),
        };
    }

    // Domain (Compra) → VM (formulario de creación)
    public static CrearFacturaViewModel ToCrearVM(this Dom.Compra compra)
    {
        var vm = new CrearFacturaViewModel
        {
            IdCompra = compra.IdCompra,
            IdProveedor = compra.Proveedor.IdProveedor,
            RazonSocial = compra.Proveedor?.RazonSocial ?? "Desconocido",
            FechaEmision = DateTime.Now,
            TipoCondicion = "Contado",
            DiasPago = 0,
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
        }

        vm.Detalles = compra
            .Detalles.Select(d => new CrearFacturaDetalleViewModel
            {
                IdProducto = d.Producto.IdProducto,
                NombreProducto = d.Producto?.Nombre ?? "Producto",
                Cantidad = d.Cantidad,
                PrecioBruto = d.PrecioPactado,
                PorcentajeDescuento = 0,
                PrecioNeto = d.PrecioPactado,
            })
            .ToList();

        return vm;
    }

    // Domain → VM (formulario de edición)
    public static ActualizarFacturaViewModel ToEditVM(this Dom.Factura factura)
    {
        var vm = new ActualizarFacturaViewModel
        {
            IdFactura = factura.IdFactura,
            IdCompra = factura.Compra?.IdCompra ?? 0,
            IdProveedor = factura.Proveedor?.IdProveedor ?? 0,
            RazonSocial = factura.Proveedor?.RazonSocial ?? "Desconocido",
            NumeroFactura = factura.Numero,
            FechaEmision = factura.FechaEmision,
            TipoCondicion = "Contado",
            DiasPago = 0,
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

        vm.Detalles = factura
            .Detalles.Select(d => new CrearFacturaDetalleViewModel
            {
                IdProducto = d.Producto.IdProducto,
                NombreProducto = d.Producto?.Nombre ?? "Producto",
                Cantidad = d.Cantidad,
                PrecioBruto = d.PrecioBruto,
                PorcentajeDescuento = d.PorcentajeDescuento,
                PrecioNeto = d.PrecioNeto,
            })
            .ToList();

        return vm;
    }

    // VM → Domain (crear)
    public static Dom.Factura ToDomain(this CrearFacturaViewModel vm)
    {
        var factura = new Dom.Factura
        {
            Compra = new Dom.Compra { IdCompra = vm.IdCompra },
            Proveedor = new Dom.Proveedor { IdProveedor = (short)vm.IdProveedor },
            Numero = vm.NumeroFactura,
            FechaEmision = DateTime.SpecifyKind(vm.FechaEmision, DateTimeKind.Utc),
            CondicionPago = ResolverCondicion(
                vm.TipoCondicion,
                vm.DiasPago,
                vm.CantidadCuotas,
                vm.InteresPorcentual
            ),
            Detalles = vm
                .Detalles.Select(d => new Dom.DetalleFactura
                {
                    Producto = new Dom.Producto { IdProducto = (short)d.IdProducto },
                    Cantidad = d.Cantidad,
                    PrecioBruto = d.PrecioBruto,
                    PorcentajeDescuento = d.PorcentajeDescuento,
                    PrecioNeto = d.PrecioNeto,
                })
                .ToList(),
        };

        return factura;
    }

    // VM → Domain (actualizar)
    public static Dom.Factura ToDomain(this ActualizarFacturaViewModel vm)
    {
        var factura = new Dom.Factura
        {
            IdFactura = vm.IdFactura,
            Numero = vm.NumeroFactura,
            FechaEmision = DateTime.SpecifyKind(vm.FechaEmision, DateTimeKind.Utc),
            CondicionPago = ResolverCondicion(
                vm.TipoCondicion,
                vm.DiasPago,
                vm.CantidadCuotas,
                vm.InteresPorcentual
            ),
            Detalles = vm
                .Detalles.Select(d => new Dom.DetalleFactura
                {
                    Producto = new Dom.Producto { IdProducto = (short)d.IdProducto },
                    Cantidad = d.Cantidad,
                    PrecioBruto = d.PrecioBruto,
                    PorcentajeDescuento = d.PorcentajeDescuento,
                    PrecioNeto = d.PrecioNeto,
                })
                .ToList(),
        };

        return factura;
    }

    // Contract → VM (documentos asociados)
    public static DocumentosRelacionadosViewModel ToDocumentosVM(
        this DocumentosRelacionadosData data
    )
    {
        return new DocumentosRelacionadosViewModel
        {
            IdFactura = data.Factura.IdFactura,
            NumeroFactura = data.Factura.Numero,
            Proveedor = data.Factura.Proveedor?.RazonSocial ?? "Desconocido",
            Comprobantes = data
                .Comprobantes.Select(c => new ResumenComprobanteViewModel
                {
                    IdComprobante = c.IdComprobante,
                    Numero = c.Numero,
                    FechaEmision = c.FechaEmision,
                    Total = c.Total,
                    Comentario = c.Comentario,
                    Motivo = c.Motivo?.Descripcion ?? "Sin motivo",
                    Tipo =
                        c is Dom.NotaCredito ? "Nota de Crédito"
                        : c is Dom.NotaDebito ? "Nota de Débito"
                        : "Comprobante",
                })
                .ToList(),
            OrdenesPago = data
                .OrdenesPago.Select(op => new ResumenOrdenPagoViewModel
                {
                    IdOrdenPago = op.IdOrdenPago,
                    Numero = op.Numero,
                    FechaPago = op.FechaPago,
                    TotalOrden = op.MontoTotal,
                    Enviada = op.Enviada,
                    MontoAplicado = op
                        .Detalles.Where(d => d.IdFactura == data.Factura.IdFactura)
                        .Sum(d => d.MontoAplicado),
                })
                .ToList(),
        };
    }

    // Helper privado para resolver condición de pago
    private static Dom.CondicionDePago ResolverCondicion(
        string tipo,
        short diasPago,
        short? cuotas,
        decimal? interes
    )
    {
        if (tipo == "Cuota")
        {
            return new Dom.Cuota
            {
                DiasPago = diasPago,
                Cuotas = cuotas ?? 1,
                InteresPorcentual = interes ?? 0,
            };
        }

        return new Dom.Contado { DiasPago = diasPago };
    }
}
