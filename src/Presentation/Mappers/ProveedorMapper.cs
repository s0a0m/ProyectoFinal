using src.Core.Contracts;
using src.Presentation.ViewModels.CuentaCorrienteVM;
using src.ViewModels;
using Dom = src.Models.Domain;

namespace src.Presentation.Mappers;

public static class ProveedorMapper
{
    // Domain → VM (crear)
    public static CrearProveedorViewModel ToCrearVM(this Dom.Proveedor proveedor)
    {
        var vm = new CrearProveedorViewModel
        {
            Cuit = proveedor.Cuit ?? string.Empty,
            RazonSocial = proveedor.RazonSocial ?? string.Empty,
            Telefono = proveedor.Telefono ?? string.Empty,
            Correo = proveedor.Correo ?? string.Empty,
            PersonaResponsable = proveedor.PersonaResponsable ?? string.Empty,
            Saldo = proveedor.SaldoInicial,
        };

        // Mapear Dirección
        if (proveedor.Direccion != null)
        {
            vm.Direccion = new DireccionViewModel
            {
                calle = proveedor.Direccion.Calle ?? string.Empty,
                numero = proveedor.Direccion.Numero,
                piso = proveedor.Direccion.Piso,
                comentario = proveedor.Direccion.Comentario ?? string.Empty,
                provincia = new ProvinciaViewModel
                {
                    Id_provincia = proveedor.Direccion.Prov?.IdProvincia ?? 0,
                },
            };
        }

        // Mapear Condición de Pago
        if (proveedor.Condicion != null)
        {
            vm.CondicionPago = new CondicionDePagoViewModel
            {
                DiasPago = proveedor.Condicion.DiasPago,
                Tipo = proveedor.Condicion is Dom.Cuota ? "Cuota" : "Contado",
                NumeroCuotas = (proveedor.Condicion as Dom.Cuota)?.Cuotas ?? 0,
                InteresPorcentual = (proveedor.Condicion as Dom.Cuota)?.InteresPorcentual ?? 0M,
            };
        }

        return vm;
    }

    // Domain → VM (actualizar)
    public static ActualizarProveedorViewModel ToActualizarVM(this Dom.Proveedor proveedor)
    {
        var vm = new ActualizarProveedorViewModel
        {
            IdProveedor = proveedor.IdProveedor,
            Cuit = proveedor.Cuit ?? string.Empty,
            RazonSocial = proveedor.RazonSocial ?? string.Empty,
            Telefono = proveedor.Telefono ?? string.Empty,
            Correo = proveedor.Correo ?? string.Empty,
            PersonaResponsable = proveedor.PersonaResponsable ?? string.Empty,
            Saldo = proveedor.SaldoInicial,
        };

        // Mapear Dirección
        if (proveedor.Direccion != null)
        {
            vm.Direccion = new DireccionViewModel
            {
                calle = proveedor.Direccion.Calle ?? string.Empty,
                numero = proveedor.Direccion.Numero,
                piso = proveedor.Direccion.Piso,
                comentario = proveedor.Direccion.Comentario ?? string.Empty,
                provincia = new ProvinciaViewModel
                {
                    Id_provincia = proveedor.Direccion.Prov?.IdProvincia ?? 0,
                },
            };
        }

        // Mapear Condición de Pago
        if (proveedor.Condicion != null)
        {
            var cuota = proveedor.Condicion as Dom.Cuota;
            vm.CondicionPago = new CondicionDePagoViewModel
            {
                DiasPago = proveedor.Condicion.DiasPago,
                Tipo = proveedor.Condicion is Dom.Cuota ? "Cuota" : "Contado",
                NumeroCuotas = cuota?.Cuotas ?? 0,
                InteresPorcentual = cuota?.InteresPorcentual ?? 0M,
            };
        }

        return vm;
    }

    // VM → Domain (crear)
    public static Dom.Proveedor ToDomain(this CrearProveedorViewModel vm)
    {
        var proveedor = new Dom.Proveedor
        {
            Cuit = vm.Cuit ?? string.Empty,
            Telefono = vm.Telefono ?? string.Empty,
            Correo = vm.Correo ?? string.Empty,
            PersonaResponsable = vm.PersonaResponsable ?? string.Empty,
            SaldoInicial = vm.Saldo,
            RazonSocial = vm.RazonSocial ?? string.Empty,
        };

        // Mapear Dirección
        if (vm.Direccion != null)
        {
            proveedor.Direccion = new Dom.Direccion
            {
                Calle = vm.Direccion.calle ?? string.Empty,
                Comentario = vm.Direccion.comentario ?? string.Empty,
                Numero = vm.Direccion.numero,
                Piso = vm.Direccion.piso,
                Prov = new Dom.Provincia
                {
                    IdProvincia = vm.Direccion.provincia?.Id_provincia ?? 0,
                },
            };
        }

        // Mapear Condición de Pago
        if (vm.CondicionPago != null)
        {
            proveedor.Condicion = ResolverCondicion(
                vm.CondicionPago.Tipo,
                vm.CondicionPago.DiasPago,
                vm.CondicionPago.NumeroCuotas,
                vm.CondicionPago.InteresPorcentual
            );
        }

        return proveedor;
    }

    // VM → Domain (actualizar)
    public static Dom.Proveedor ToDomain(this ActualizarProveedorViewModel vm)
    {
        var proveedor = new Dom.Proveedor
        {
            IdProveedor = vm.IdProveedor,
            Cuit = vm.Cuit ?? string.Empty,
            Telefono = vm.Telefono ?? string.Empty,
            Correo = vm.Correo ?? string.Empty,
            PersonaResponsable = vm.PersonaResponsable ?? string.Empty,
            SaldoInicial = vm.Saldo,
            RazonSocial = vm.RazonSocial ?? string.Empty,
        };

        // Mapear Dirección
        if (vm.Direccion != null)
        {
            proveedor.Direccion = new Dom.Direccion
            {
                Calle = vm.Direccion.calle ?? string.Empty,
                Comentario = vm.Direccion.comentario ?? string.Empty,
                Numero = vm.Direccion.numero,
                Piso = vm.Direccion.piso,
                Prov = new Dom.Provincia
                {
                    IdProvincia = vm.Direccion.provincia?.Id_provincia ?? 0,
                },
            };
        }

        // Mapear Condición de Pago
        if (vm.CondicionPago != null)
        {
            proveedor.Condicion = ResolverCondicion(
                vm.CondicionPago.Tipo,
                vm.CondicionPago.DiasPago,
                vm.CondicionPago.NumeroCuotas,
                vm.CondicionPago.InteresPorcentual
            );
        }

        return proveedor;
    }

    // Preparar VM con lista de provincias
    public static CrearProveedorViewModel PrepareWithProvincias(
        this CrearProveedorViewModel vm,
        IEnumerable<Dom.Provincia> provincias
    )
    {
        if (vm.Direccion != null)
        {
            vm.Direccion.ListaProvincias = provincias?.ToList() ?? new List<Dom.Provincia>();
        }
        return vm;
    }

    public static ActualizarProveedorViewModel PrepareWithProvincias(
        this ActualizarProveedorViewModel vm,
        IEnumerable<Dom.Provincia> provincias
    )
    {
        if (vm.Direccion != null)
        {
            vm.Direccion.ListaProvincias = provincias?.ToList() ?? new List<Dom.Provincia>();
        }
        return vm;
    }

    // Cuenta Corriente
    public static CuentaCorrienteVM ToCuentaCorrienteVM(this CuentaCorrienteData data)
    {
        var vm = new CuentaCorrienteVM
        {
            IdProveedor = data.Proveedor.IdProveedor,
            RazonSocial = data.Proveedor.RazonSocial ?? string.Empty,
            PersonaResponsable = data.Proveedor.PersonaResponsable ?? string.Empty,
            Cuit = data.Proveedor.Cuit ?? string.Empty,
            SaldoTotal = data.Proveedor.SaldoActual,
            TotalFacturado = data.Facturas?.Sum(f => f.TotalFacturado) ?? 0,
            TotalNC = data.Comprobantes?.Where(c => c is Dom.NotaCredito).Sum(c => c.Total) ?? 0,
            TotalND = data.Comprobantes?.Where(c => c is Dom.NotaDebito).Sum(c => c.Total) ?? 0,
        };

        // Calcular total pagado desde ordenes confirmadas
        vm.TotalPagado = data.OrdenesPago?.Sum(o => o.MontoTotal) ?? 0;

        vm.MotivosNC =
            data.MotivosNC?.Select(m => new MotivoCCVM
                {
                    IdMotivo = m.IdMotivo,
                    Descripcion = m.Descripcion ?? string.Empty,
                })
                .ToList()
            ?? new List<MotivoCCVM>();

        vm.MotivosND =
            data.MotivosND?.Select(m => new MotivoCCVM
                {
                    IdMotivo = m.IdMotivo,
                    Descripcion = m.Descripcion ?? string.Empty,
                })
                .ToList()
            ?? new List<MotivoCCVM>();

        foreach (
            var factura in data.Facturas?.OrderByDescending(f => f.FechaEmision)
                ?? Enumerable.Empty<Dom.Factura>()
        )
        {
            // Comprobantes de esta factura
            var notasFactura =
                data.Comprobantes?.Where(c => c.IdFacturaReferencia == factura.IdFactura).ToList()
                ?? new List<Dom.Comprobante>();

            // Pagos de esta factura
            var pagosFactura =
                data.OrdenesPago?.Where(o =>
                        o.Detalles != null && o.Detalles.Any(d => d.IdFactura == factura.IdFactura)
                    )
                    .ToList()
                ?? new List<Dom.OrdenPago>();

            var facturaVM = new FacturaCCVM
            {
                IdFactura = factura.IdFactura,
                NumeroFactura = factura.Numero ?? string.Empty,
                FechaEmision = factura.FechaEmision,
                TotalFacturado = factura.TotalFacturado,
                Saldo = factura.Saldo,
                Pagada = factura.Pagada,
                PuedeEditarse = factura.PuedeEditarse && !notasFactura.Any() && !pagosFactura.Any(),
                Notas = notasFactura
                    .Select(c => new NotaCCVM
                    {
                        IdComprobante = c.IdComprobante,
                        Tipo = c is Dom.NotaCredito ? "NC" : "ND",
                        Numero = c.Numero ?? string.Empty,
                        Motivo = c.Motivo?.Descripcion ?? string.Empty,
                        Total = c.Total,
                        FechaEmision = c.FechaEmision,
                    })
                    .ToList(),
                Pagos = pagosFactura
                    .Select(o => new PagoCCVM
                    {
                        IdOrdenPago = o.IdOrdenPago,
                        Numero = o.Numero ?? string.Empty,
                        FechaPago = o.FechaPago,
                        Enviada = o.Enviada,
                        MontoAplicado =
                            o.Detalles?.Where(d => d.IdFactura == factura.IdFactura)
                                .Sum(d => d.MontoAplicado)
                            ?? 0,
                    })
                    .ToList(),
            };

            vm.Facturas.Add(facturaVM);
        }

        return vm;
    }

    // Helper privado para resolver condición de pago
    private static Dom.CondicionDePago? ResolverCondicion(
        string? tipo,
        short diasPago,
        short numeroCuotas,
        decimal interesPorcentual
    )
    {
        if (tipo == "Cuota")
        {
            return new Dom.Cuota
            {
                DiasPago = diasPago,
                Cuotas = numeroCuotas,
                InteresPorcentual = interesPorcentual,
            };
        }

        if (tipo == "Contado")
        {
            return new Dom.Contado { DiasPago = diasPago };
        }

        return null;
    }
}
