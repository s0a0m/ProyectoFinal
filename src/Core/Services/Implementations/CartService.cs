using System.Text.Json;
using Core.Common;
using Microsoft.AspNetCore.Http;
using src.Core.Contracts;
using src.Core.Services.Interfaces;
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

        private ISession Session => _httpContextAccessor.HttpContext!.Session;

        public async Task<ServiceResult<List<CarritoItemDto>>> ObtenerCarritoCompletoAsync()
        {
            var sessionData = Session.GetString(SESSION_KEY);
            if (string.IsNullOrEmpty(sessionData))
                return ServiceResult<List<CarritoItemDto>>.Ok(new List<CarritoItemDto>());

            var carrito = JsonSerializer.Deserialize<List<CarritoItemDto>>(sessionData)
                   ?? new List<CarritoItemDto>();

            return ServiceResult<List<CarritoItemDto>>.Ok(carrito);
        }

        private void GuardarCarritoEnSesion(List<CarritoItemDto> carrito)
        {
            var options = new JsonSerializerOptions { WriteIndented = false };
            Session.SetString(SESSION_KEY, JsonSerializer.Serialize(carrito, options));
        }

        public async Task<ServiceResult> ActualizarCantidadAsync(short idProducto, short idProveedor, int cantidad)
        {
            var result = await ObtenerCarritoCompletoAsync();
            if (!result.Success)
                return ServiceResult.Fail(result.Message);

            var carrito = result.Data;
            var item = carrito.FirstOrDefault(x =>
                x.IdProducto == idProducto && x.IdProveedor == idProveedor);

            if (item == null)
                return ServiceResult.Fail("El producto no se encontró en el carrito.");

            item.Cantidad = cantidad;
            GuardarCarritoEnSesion(carrito);
            return ServiceResult.Ok("Cantidad actualizada correctamente.");
        }

        public async Task<ServiceResult> AgregarItemAsync(CarritoItemDto nuevoItem)
        {
            var result = await ObtenerCarritoCompletoAsync();
            if (!result.Success)
                return ServiceResult.Fail(result.Message);

            var carrito = result.Data;
            var itemExistente = carrito.FirstOrDefault(x =>
                x.IdProducto == nuevoItem.IdProducto && x.IdProveedor == nuevoItem.IdProveedor);

            if (itemExistente != null)
                itemExistente.Cantidad += nuevoItem.Cantidad;
            else
                carrito.Add(nuevoItem);

            GuardarCarritoEnSesion(carrito);
            return ServiceResult.Ok("Item agregado correctamente.");
        }

        public async Task<ServiceResult> RemoverItemAsync(short idProducto, short idProveedor)
        {
            var result = await ObtenerCarritoCompletoAsync();
            if (!result.Success)
                return ServiceResult.Fail(result.Message);

            var carrito = result.Data;
            var item = carrito.FirstOrDefault(x =>
                x.IdProducto == idProducto && x.IdProveedor == idProveedor);

            if (item != null)
            {
                carrito.Remove(item);
                GuardarCarritoEnSesion(carrito);
            }

            return ServiceResult.Ok("Item removido correctamente.");
        }

        public async Task<ServiceResult<List<CarritoItemDto>>> ObtenerItemsPorProveedorAsync(short idProveedor)
        {
            var result = await ObtenerCarritoCompletoAsync();
            if (!result.Success)
                return ServiceResult<List<CarritoItemDto>>.Fail(result.Message);

            var items = result.Data.Where(x => x.IdProveedor == idProveedor).ToList();
            return ServiceResult<List<CarritoItemDto>>.Ok(items);
        }

        public async Task<ServiceResult> LimpiarCarritoPorProveedorAsync(short idProveedor)
        {
            var result = await ObtenerCarritoCompletoAsync();
            if (!result.Success)
                return ServiceResult.Fail(result.Message);

            var carrito = result.Data;
            carrito.RemoveAll(x => x.IdProveedor == idProveedor);
            GuardarCarritoEnSesion(carrito);
            return ServiceResult.Ok("Carrito limpiado correctamente.");
        }

        public async Task<int> GetCantidadTotalItemsAsync()
        {
            var result = await ObtenerCarritoCompletoAsync();
            return result.Success ? result.Data.Count : 0;
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
            var item = new CarritoItemDto
            {
                IdProducto       = idProducto,
                NombreProducto   = relacion.Producto?.Nombre ?? "Producto Desconocido",
                CodigosExternos  = dto.CodigosBarrasExternos ?? new List<string>(),
                IdProveedor      = idProveedor,
                NombreProveedor  = relacion.Proveedor?.RazonSocial ?? "Proveedor Desconocido",
                PrecioUnitario   = relacion.Precio,
                Cantidad         = cantidad
            };

            return await AgregarItemAsync(item);
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

            var result = await ObtenerCarritoCompletoAsync();
            if (!result.Success)
                return ServiceResult.Fail(result.Message);

            var carrito = result.Data;
            var item = carrito.FirstOrDefault(x =>
                x.IdProducto == idProducto && x.IdProveedor == idProveedor);

            if (item == null)
                return ServiceResult.Fail("El producto no se encontró en su carrito.");

            item.Cantidad = cantidad;
            GuardarCarritoEnSesion(carrito);
            return ServiceResult.Ok("Cantidad actualizada correctamente.");
        }

        public async Task<ServiceResult> ValidarStockParaPrevisualizarAsync(short idProveedor)
        {
            var result = await ObtenerItemsPorProveedorAsync(idProveedor);
            if (!result.Success)
                return ServiceResult.Fail(result.Message);

            var items = result.Data;

            if (!items.Any())
                return ServiceResult.Fail("No hay productos en el carrito para este proveedor.");

            var errores = new List<string>();

            foreach (var item in items)
            {
                var dto = await _prodProvRepo.GetByIdAsync(item.IdProducto, item.IdProveedor);

                if (dto == null)
                {
                    errores.Add($"'{item.NombreProducto}' ya no está disponible con este proveedor.");
                    continue;
                }

                var stockActual = dto.ProductoProveedor.StockAsignado;

                if (item.Cantidad > stockActual)
                {
                    errores.Add(
                        $"'{item.NombreProducto}': solicitás {item.Cantidad} unidades " +
                        $"pero el stock disponible es {stockActual}."
                    );
                }
            }

            if (errores.Any())
                return ServiceResult.Fail(string.Join(" | ", errores));

            return ServiceResult.Ok();
        }
    }
}
