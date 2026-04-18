using src.Core.Contracts;
using src.Presentation.ViewModels.CarritoVM;

namespace src.Presentation.Mappers;

public static class CarritoMapper
{
    public static CarritoItemViewModel ToViewModel(this CarritoItemDto dto)
    {
        return new CarritoItemViewModel
        {
            IdProducto = dto.IdProducto,
            NombreProducto = dto.NombreProducto,
            CodigosExternos = dto.CodigosExternos,
            IdProveedor = dto.IdProveedor,
            NombreProveedor = dto.NombreProveedor,
            PrecioUnitario = dto.PrecioUnitario,
            Cantidad = dto.Cantidad,
        };
    }
}
