using src.Core.Services.Interfaces;
using src.Repositories.Interfaces;
using src.Presentation.ViewModels.ProductoVM;
using Dom = src.Models.Domain;
using System.Linq;
using ZXing;
using src.Models.CodeFirst;

namespace src.Core.Services.Implementations
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepo;
        private readonly ICategoriaRepository _categoriaRepo;
        private readonly IBarcodeAdapter _barcoRepo;
        public ProductoService(IProductoRepository productoRepo, ICategoriaRepository categoriaRepo,IBarcodeAdapter barco)
        {
            _productoRepo = productoRepo;
            _categoriaRepo = categoriaRepo;
            _barcoRepo = barco;
        }

        public async Task<IEnumerable<ProductoListarViewModel>> GetAllParaListadoAsync()
        {
            var productos = await _productoRepo.GetAllAsync();

           
            return productos.Select(p => new ProductoListarViewModel
            {
                IdProducto = p.IdProducto,
                Nombre = p.Nombre,
                StockTotal = p.StockTotal,
                StockMinimo = p.StockMinimo,
                Activo = p.Activo,
                // Aplanamos las listas para búsqueda fácil en el string
                CodigosBarra = string.Join(", ", p.CodigoBarra.Select(cb => cb.Codigo)),
                Categorias = string.Join(", ", p.Categoria.Select(c => c.Nombre))
            });
        }

        public async Task<IEnumerable<ProductoListarViewModel>> GetProductosBajoStockAsync()
        {
            var todos = await GetAllParaListadoAsync();
            // RF 2.8 - Filtrar lógica de negocio
            return todos.Where(p => p.EnAlertaStock);
        }

        public async Task<CrearProductoViewModel> PrepararCrearViewModelAsync()
        {
            var vm = new CrearProductoViewModel();
            await RepoblarViewModelAsync(vm);
            return vm;
        }

        public async Task<ActualizarProductoViewModel> PrepararActualizarViewModelAsync(int id)
        {
            var producto = await _productoRepo.GetByIdAsync(id);
            if (producto == null) throw new KeyNotFoundException("Producto no encontrado");

            var vm = new ActualizarProductoViewModel
            {
                IdProducto = producto.IdProducto,
                Nombre = producto.Nombre,
                StockMinimo = producto.StockMinimo,
                StockTotal = producto.StockTotal,
                Activo = producto.Activo,
                // Cargamos los IDs y Strings existentes
                CodigosBarra = producto.CodigoBarra.Select(cb => cb.Codigo).ToList(),
                IdsCategoriasSeleccionadas = producto.Categoria.Select(c => (short)c.IdCategoria).ToList(),
                CodigosRegistradosEnBd = producto.CodigoBarra.Select(cb => cb.Codigo).ToList()
            };
            await RepoblarViewModelAsync(vm); // Cargar lista de categorías
            return vm;
        }

        public async Task CreateAsync(CrearProductoViewModel vm)
        {
            var codigosFinales = new List<string>();

            string paramA = vm.Nombre.Length >= 5 ? vm.Nombre.Substring(0, 5).ToUpper() : vm.Nombre.ToUpper();
            string paramB = DateTime.Now.ToString("yyMMdd");
            // Parámetro C: Random alfanumérico
            string paramC = GenerarStringRandom(5); 
            string codigoInterno = _barcoRepo.FormatearCodigo128( paramA, paramB,paramC);
                _barcoRepo.ValidarFormatoCodigo128Async(codigoInterno);
            codigosFinales.Add(codigoInterno);
            
             // codigos manuales
            if (vm.CodigosBarra != null)
            {
                codigosFinales.AddRange(vm.CodigosBarra.Where(c => !string.IsNullOrWhiteSpace(c)));
            }

            if (!codigosFinales.Any())
            {
                throw new ArgumentException("Debe generar un código interno o agregar uno manual.");
            }

            foreach (var codigo in codigosFinales)
            {
                if (await _productoRepo.ExistsCodigoBarraAsync(codigo)) 
                {
                    throw new ArgumentException($"El código {codigo} ya está asignado a otro producto.");
                }
            }

            var nuevoProducto = new Dom.Producto
            {
                Nombre = vm.Nombre.Trim(),
                StockMinimo = vm.StockMinimo,
                StockTotal = vm.StockTotal,
                Activo = true,
                Categoria = vm.IdsCategoriasSeleccionadas
                              .Select(id => new Dom.Categoria { IdCategoria = id })
                              .ToList(),
                CodigoBarra = codigosFinales.Select(c => new Dom.CodigoBarra { Codigo = c }).ToList()
            };

            await _productoRepo.AddAsync(nuevoProducto);
        }


        public async Task UpdateAsync(ActualizarProductoViewModel vm)
        {
            if (vm.CodigosBarra == null || !vm.CodigosBarra.Any(c => !string.IsNullOrWhiteSpace(c)))
            {
                throw new ArgumentException("El producto debe tener al menos un código de barras asociado.");
            }

           foreach (var codigo in vm.CodigosBarra)
            {
                if (await _productoRepo.ExistsCodigoBarraAsync(codigo, vm.IdProducto)) 
                {
                    throw new ArgumentException($"El código {codigo} ya está asignado a otro producto.");
                }
            }

            var productoActualizado = new Dom.Producto
            {
                IdProducto = vm.IdProducto,
                Nombre = vm.Nombre.Trim(),
                StockMinimo = vm.StockMinimo,
                StockTotal = vm.StockTotal,
                Activo = vm.Activo,
                Categoria = vm.IdsCategoriasSeleccionadas
                              .Select(id => new Dom.Categoria { IdCategoria = id })
                              .ToList(),
                CodigoBarra = vm.CodigosBarra
                              .Where(c => !string.IsNullOrWhiteSpace(c))
                              .Select(code => new Dom.CodigoBarra { Codigo = code.Trim() })
                              .ToList()
            };

            await _productoRepo.UpdateAsync(productoActualizado);
        }

        public async Task DeleteAsync(int id)
        {
            await _productoRepo.DeleteAsync(id);
        }

        public async Task RepoblarViewModelAsync(CrearProductoViewModel vm)
        {
            // Incluimos Familia para poder leer su nombre
            // (Asegúrate que tu repo o query incluya .Include(c => c.Familia))
            var categorias = await _categoriaRepo.GetAllAsync(); 
            
            vm.ListaCategoriasDisponibles = categorias.Select(c => new CategoriaOpcionDto 
            { 
                Id = c.IdCategoria, 
                Nombre = c.Nombre,
                // Aquí manejamos la lógica del nombre:
                Familia = c.Familia?.Nombre ?? "Sin Familia" 
            }).OrderBy(c => c.Familia).ThenBy(c => c.Nombre);
        }
                // Sobrecarga para Actualizar
        public async Task RepoblarViewModelAsync(ActualizarProductoViewModel vm)
        {
            // 1. Cargar Categorías (lo que ya hacías)
            var categorias = await _categoriaRepo.GetAllAsync();
            vm.ListaCategoriasDisponibles = categorias.Select(c => new CategoriaOpcionDto 
            { 
                Id = c.IdCategoria, 
                Nombre = c.Nombre,
                Familia = c.Familia?.Nombre ?? "Sin Familia" 
            }).OrderBy(c => c.Familia).ThenBy(c => c.Nombre);

            // 2. NUEVO: Cargar los códigos que REALMENTE existen en la BD
            var productoEnBd = await _productoRepo.GetByIdAsync(vm.IdProducto);
            if (productoEnBd != null)
            {
                vm.CodigosRegistradosEnBd = productoEnBd.CodigoBarra.Select(c => c.Codigo).ToList();
            }
        }

        private string GenerarStringRandom(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}