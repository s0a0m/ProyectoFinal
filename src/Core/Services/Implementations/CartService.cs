using System.Text.Json;
using Microsoft.AspNetCore.Http;
using src.Core.Services.Interfaces;
using src.Presentation.ViewModels.CarritoVM;

namespace src.Core.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string SESSION_KEY = "CarritoCompras";

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Helper privado para leer/escribir sesión
        private ISession Session => _httpContextAccessor.HttpContext!.Session;

        public async Task<List<CarritoItemViewModel>> ObtenerCarritoCompletoAsync()
        {
            var sessionData = Session.GetString(SESSION_KEY);
            if (string.IsNullOrEmpty(sessionData))
            {
                return new List<CarritoItemViewModel>();
            }
            return JsonSerializer.Deserialize<List<CarritoItemViewModel>>(sessionData) ?? new List<CarritoItemViewModel>();
        }

        private void GuardarCarritoEnSesion(List<CarritoItemViewModel> carrito)
        {
            var options = new JsonSerializerOptions { WriteIndented = false };
            var json = JsonSerializer.Serialize(carrito, options);
            Session.SetString(SESSION_KEY, json);
        }

        public async Task AgregarItemAsync(CarritoItemViewModel nuevoItem)
        {
            var carrito = await ObtenerCarritoCompletoAsync();

            // Buscamos si ya existe el producto PARA ESE PROVEEDOR
            var itemExistente = carrito.FirstOrDefault(x => 
                x.IdProducto == nuevoItem.IdProducto && 
                x.IdProveedor == nuevoItem.IdProveedor);

            if (itemExistente != null)
            {
                // Si existe, sumamos cantidad
                itemExistente.Cantidad += nuevoItem.Cantidad;
            }
            else
            {
                // Si no, lo agregamos
                carrito.Add(nuevoItem);
            }

            GuardarCarritoEnSesion(carrito);
        }

        public async Task RemoverItemAsync(short idProducto, short idProveedor)
        {
            var carrito = await ObtenerCarritoCompletoAsync();
            var item = carrito.FirstOrDefault(x => x.IdProducto == idProducto && x.IdProveedor == idProveedor);

            if (item != null)
            {
                carrito.Remove(item);
                GuardarCarritoEnSesion(carrito);
            }
        }

        public async Task<List<CarritoItemViewModel>> ObtenerItemsPorProveedorAsync(short idProveedor)
        {
            var carrito = await ObtenerCarritoCompletoAsync();
            return carrito.Where(x => x.IdProveedor == idProveedor).ToList();
        }

        public async Task LimpiarCarritoPorProveedorAsync(short idProveedor)
        {
            var carrito = await ObtenerCarritoCompletoAsync();
            
            // Removemos todos los items que coincidan con ese proveedor
            carrito.RemoveAll(x => x.IdProveedor == idProveedor);
            
            GuardarCarritoEnSesion(carrito);
        }

        public async Task<int> GetCantidadTotalItemsAsync()
        {
            var carrito = await ObtenerCarritoCompletoAsync();
            return carrito.Sum(x => x.Cantidad);
        }
    }
}