using src.Presentation.ViewModels.CompraVM;
using Dom = src.Models.Domain;

namespace src.Presentation.Mappers;

public static class CompraMapper
{
    // Domain → VM (Para la vista de Gestión)
    public static GestionarCompraViewModel ToGestionarVM(this Dom.Compra compra)
    {
        return new GestionarCompraViewModel
        {
            IdCompra = compra.IdCompra,
            Fecha = compra.FechaCompra,
            Estado = compra.Estado.ToString(),
            IdProveedor = compra.Proveedor.IdProveedor,
            ProveedorRazonSocial = compra.Proveedor?.RazonSocial ?? "Desconocido",
            UsuarioNombreCompleto = $"{compra.Usuario?.Nombre} {compra.Usuario?.Apellido}",
            Observaciones = compra.Observaciones,
            Detalles = compra
                .Detalles.Select(d => new GestionarDetalleViewModel
                {
                    IdDetalleCompra = d.IdDetalleCompra,
                    IdProducto = d.Producto?.IdProducto ?? 0,
                    ProductoNombre = d.Producto?.Nombre ?? "Producto",
                    Cantidad = d.Cantidad,
                    PrecioPactado = d.PrecioPactado,
                    Eliminar = false,
                })
                .ToList(),
        };
    }

    // VM (Gestionar) → Domain (Para actualizar)
    public static Dom.Compra ToDomain(this GestionarCompraViewModel vm)
    {
        return new Dom.Compra
        {
            IdCompra = vm.IdCompra,
            Observaciones = vm.Observaciones,
            Proveedor = new Dom.Proveedor { IdProveedor = vm.IdProveedor },
            Detalles = vm
                .Detalles.Where(d => !d.Eliminar)
                .Select(d => new Dom.DetalleCompra
                {
                    IdDetalleCompra = d.IdDetalleCompra,
                    Producto = new Dom.Producto { IdProducto = d.IdProducto },
                    Cantidad = d.Cantidad,
                    PrecioPactado = d.PrecioPactado,
                })
                .ToList(),
        };
    }

    // VM (Confirmar) → Domain (Para crear)
    public static Dom.Compra ToDomain(this ConfirmarCompraViewModel vm)
    {
        return new Dom.Compra
        {
            FechaCompra = vm.FechaCompra,
            Observaciones = vm.Observaciones,
            Proveedor = new Dom.Proveedor { IdProveedor = vm.IdProveedor },
            Detalles = vm
                .Items.Select(i => new Dom.DetalleCompra
                {
                    Producto = new Dom.Producto { IdProducto = i.IdProducto },
                    Cantidad = i.Cantidad,
                    PrecioPactado = i.PrecioUnitario,
                })
                .ToList(),
        };
    }
}
