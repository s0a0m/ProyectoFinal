using Core.Common;
using src.Core.Contracts;

namespace src.Core.Services.Interfaces
{
    public interface ICartService
    {
        // Operaciones Básicas
        Task<ServiceResult> AgregarItemAsync(CarritoItemDto item);
        Task<ServiceResult> RemoverItemAsync(short idProducto, short idProveedor);
        Task<ServiceResult<List<CarritoItemDto>>> ObtenerCarritoCompletoAsync();

        // Operaciones de Negocio
        Task<int> GetCantidadTotalItemsAsync();
        Task<ServiceResult<List<CarritoItemDto>>> ObtenerItemsPorProveedorAsync(short idProveedor);
        Task<ServiceResult> LimpiarCarritoPorProveedorAsync(short idProveedor);
        Task<ServiceResult> ActualizarCantidadAsync(short idProducto, short idProveedor, int cantidad);
        Task<ServiceResult> ValidarYAgregarItemAsync(short idProducto, short idProveedor, int cantidad);
        Task<ServiceResult> ValidarYActualizarCantidadAsync(short idProducto, short idProveedor, int cantidad);
        Task<ServiceResult> ValidarStockParaPrevisualizarAsync(short idProveedor);
    }
}
