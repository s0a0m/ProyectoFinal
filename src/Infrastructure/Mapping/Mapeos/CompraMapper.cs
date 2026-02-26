using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;
using src.Presentation.ViewModels.CompraVM;
using src.Models.Common;

namespace src.Models.Mappers;

public static partial class DominioMapper
{
    [MapperIgnoreSource(nameof(EF.Compra.IdProveedor))]
    [MapperIgnoreSource(nameof(EF.Compra.IdUsuario))]
    [MapperIgnoreSource(nameof(EF.Compra.Facturas))]
    public static partial Dom.Compra Map(EF.Compra source);
    public static partial IEnumerable<Dom.Compra> Map(IEnumerable<EF.Compra> source);
    [MapperIgnoreTarget(nameof(EF.Compra.IdProveedor))]
    [MapperIgnoreTarget(nameof(EF.Compra.IdUsuario))]
    [MapperIgnoreTarget(nameof(EF.Compra.Facturas))]
    [MapperIgnoreTarget(nameof(EF.Compra.Usuario))]
    [MapperIgnoreTarget(nameof(EF.Compra.Proveedor))]

    [MapperIgnoreSource(nameof(Dom.Compra.Proveedor))]
    [MapperIgnoreSource(nameof(Dom.Compra.Usuario))]
    [MapperIgnoreSource(nameof(Dom.Compra.TotalOrden))]

    public static partial EF.Compra Map(Dom.Compra source);
    public static partial IEnumerable<EF.Compra> Map(IEnumerable<Dom.Compra> source);

    [MapProperty(nameof(CrearCompraViewModel.IdProveedor), nameof(Dom.Compra.Proveedor.IdProveedor))]
    public static partial Dom.Compra Map(CrearCompraViewModel source);

    [MapProperty(nameof(CompraDetalleViewModel.IdProducto), nameof(Dom.DetalleCompra.Producto.IdProducto))]
    public static partial Dom.DetalleCompra Map(CompraDetalleViewModel source);


    // mapear listar VM de compras

    [MapProperty("Usuario.Nombre", nameof(ListarCompraViewModel.UsuarioNombre))]
    [MapProperty("Usuario.Apellido", nameof(ListarCompraViewModel.UsuarioApellido))]
    [MapProperty("Proveedor.IdProveedor", nameof(ListarCompraViewModel.IdProveedor))]
    [MapProperty("Proveedor.RazonSocial", nameof(ListarCompraViewModel.ProveedorRazonSocial))]
    [MapProperty(nameof(Dom.Compra.FechaCompra), nameof(ListarCompraViewModel.Fecha))]
    [MapProperty(nameof(Dom.Compra.TotalOrden), nameof(ListarCompraViewModel.Total))]
    [MapProperty(nameof(Dom.Compra.Estado), nameof(ListarCompraViewModel.Estado))]
    public static partial ListarCompraViewModel MapToRead(Dom.Compra source);
    public static partial IEnumerable<ListarCompraViewModel> MapToRead(IEnumerable<Dom.Compra> source);

    [MapProperty("Producto.IdProducto", nameof(ListarDetalleCompraViewModel.IdProducto))]
    [MapProperty("Producto.Nombre", nameof(ListarDetalleCompraViewModel.ProductoNombre))]
    // [MapProperty("Producto.CodigoBarra", nameof(ListarDetalleCompraViewModel.ProductoCodigo))]

    public static partial ListarDetalleCompraViewModel MapToRead(Dom.DetalleCompra source);
    public static partial IEnumerable<ListarDetalleCompraViewModel> MapToRead(IEnumerable<Dom.DetalleCompra> source);

    // private static List<string> MapCodigos(IEnumerable<Dom.CodigoBarra> source)
    // {
    //     return source.Select(x => x.Codigo).ToList();
    // }
    private static string MapEstadoNombre(EstadoCompra estado)
    {
        var key = Enum.GetName(typeof(EstadoCompra), estado);
        if (string.IsNullOrEmpty(key))
        {
            return "INDEFINIDO";
        }
        return key;

    }
}