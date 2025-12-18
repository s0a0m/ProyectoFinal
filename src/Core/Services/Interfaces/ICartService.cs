using src.Presentation.ViewModels.CarritoVM;

namespace src.Core.Services.Interfaces
{
    public interface ICartService
    {
        // Operaciones Básicas
        Task AgregarItemAsync(CarritoItemViewModel item);
        Task RemoverItemAsync(short idProducto, short idProveedor); // Clave compuesta por si un producto lo venden 2 proveedores
        Task<List<CarritoItemViewModel>> ObtenerCarritoCompletoAsync();
        
        // Operaciones de Negocio
        Task<int> GetCantidadTotalItemsAsync();
        Task<List<CarritoItemViewModel>> ObtenerItemsPorProveedorAsync(short idProveedor);
        Task LimpiarCarritoPorProveedorAsync(short idProveedor); // Se usa al confirmar compra
    }
}