using System.Text.Json;
using Core.Common;
using Microsoft.AspNetCore.Http;
using src.Core.Services.Interfaces;
using src.Presentation.ViewModels.CarritoVM;
using src.Repositories.Interfaces;

namespace src.Core.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string SESSION_KEY = "CarritoCompras";
        private readonly IProductoProveedorRepository _prodProvRepo;

        public CartService(IHttpContextAccessor httpContextAccessor, IProductoProveedorRepository prodProvRepo)
        {
            _httpContextAccessor = httpContextAccessor;
            _prodProvRepo = prodProvRepo;
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

        public async Task ActualizarCantidadAsync(short idProducto, short idProveedor, int cantidad)
        {
            var carrito = await ObtenerCarritoCompletoAsync();

            var item = carrito.FirstOrDefault(x =>
                x.IdProducto == idProducto &&
                x.IdProveedor == idProveedor);

            if (item == null)
            {
                // Opción 1: no hacer nada (silencioso)
                return;

                // Opción 2 (más estricta):
                // throw new InvalidOperationException("El item no existe en el carrito");
            }

            item.Cantidad = cantidad;

            GuardarCarritoEnSesion(carrito);
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
            //return carrito.Sum(x => x.Cantidad);
            return carrito.Count;
        }
        public async Task<ServiceResult> ValidarYAgregarItemAsync(short idProducto, short idProveedor, int cantidad)
        {
            if (cantidad <= 0)
                return ServiceResult.Fail("La cantidad debe ser mayor a cero.");

            var dto = await _prodProvRepo.GetByIdAsync(idProducto, idProveedor);
            if (dto == null)
                return ServiceResult.Fail("El producto no está disponible con el proveedor seleccionado.");

            if (cantidad > dto.ProductoProveedor.StockAsignado)
                return ServiceResult.Fail($"Stock insuficiente. Máximo disponible: {dto.ProductoProveedor.StockAsignado}");

            var relacion = dto.ProductoProveedor;

            var item = new CarritoItemViewModel
            {
                IdProducto = idProducto,
                NombreProducto = relacion.Producto?.Nombre ?? "Producto Desconocido",
                CodigosExternos = dto.CodigosBarrasExternos ?? new List<string>(),
                IdProveedor = idProveedor,
                NombreProveedor = relacion.Proveedor?.RazonSocial ?? "Proveedor Desconocido",
                PrecioUnitario = relacion.Precio,
                Cantidad = cantidad
            };

            await AgregarItemAsync(item);
            return ServiceResult.Ok("Producto añadido al carrito correctamente.");
        }
        public async Task<ServiceResult> ValidarYActualizarCantidadAsync(short idProducto, short idProveedor, int cantidad)
        {
            if (cantidad <= 0)
                return ServiceResult.Fail("La cantidad debe ser mayor a cero para actualizar.");

            var dto = await _prodProvRepo.GetByIdAsync(idProducto, idProveedor);
            if (dto == null)
                return ServiceResult.Fail("El producto ya no está disponible con este proveedor.");

            if (cantidad > dto.ProductoProveedor.StockAsignado)
                return ServiceResult.Fail($"Stock insuficiente. El stock actual disponible es {dto.ProductoProveedor.StockAsignado}.");

            var carrito = await ObtenerCarritoCompletoAsync();
            var item = carrito.FirstOrDefault(x =>
                x.IdProducto == idProducto &&
                x.IdProveedor == idProveedor);

            if (item == null)
                return ServiceResult.Fail("El producto no se encontró en su carrito.");

            item.Cantidad = cantidad;
            GuardarCarritoEnSesion(carrito);

            return ServiceResult.Ok("Cantidad actualizada correctamente.");
        }

    }
}