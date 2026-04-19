using src.Core.Contracts;
using src.Presentation.ViewModels.CompraVM;
using Dom = src.Models.Domain;

namespace src.Presentation.Mappers;

public static class CompraMapper
{
    // Domain → ListarCompraViewModel
    public static ListarCompraViewModel ToListarVM(this Dom.Compra c)
    {
        return new ListarCompraViewModel
        {
            IdCompra = c.IdCompra,
            Fecha = c.FechaCompra,
            Estado = c.Estado.ToString(),
            Total = c.TotalOrden,
            ProveedorRazonSocial = c.Proveedor?.RazonSocial ?? "Desc.",
            UsuarioNombre = c.Usuario?.Nombre ?? "-",
            UsuarioApellido = c.Usuario?.Apellido ?? "-",
            Detalles = c
                .Detalles.Select(d => new ListarDetalleCompraViewModel
                {
                    IdDetalleCompra = d.IdDetalleCompra,
                    ProductoNombre = d.Producto?.Nombre ?? "-",
                    Cantidad = d.Cantidad,
                })
                .ToList(),
        };
    }

    // Carrito DTO → ConfirmarCompraViewModel (Previsualizar)
    public static ConfirmarCompraViewModel ToPrevisualizarVM(
        short idProveedor,
        List<CarritoItemDto> items
    )
    {
        return new ConfirmarCompraViewModel
        {
            IdProveedor = idProveedor,
            NombreProveedor = items.FirstOrDefault()?.NombreProveedor ?? "Proveedor",
            Items = items.Select(dto => dto.ToViewModel()).ToList(),
        };
    }

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
