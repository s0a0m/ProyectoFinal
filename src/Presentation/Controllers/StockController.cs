using Microsoft.AspNetCore.Mvc;
using src.Presentation.ViewModels.StockVM;
using src.Presentation.Attributes;
using src.Core.Services.Interfaces;
using src.Presentation.Services;
using src.Repositories.Interfaces;

namespace src.Controllers;

[AuthorizePermiso("P14_Gestion_Stock")]
public class StockController : Controller
{
    private readonly IStockService  _stockService;
    private readonly LayoutService  _layoutService;
    private readonly IFilaRepository _filaRepo;
    private readonly IDepositoRepository _depositoRepo;

    public StockController(
        IStockService   stockService,
        LayoutService   layoutService,
        IFilaRepository filaRepo,
        IDepositoRepository depositoRepo)
    {
        _stockService  = stockService;
        _layoutService = layoutService;
        _filaRepo      = filaRepo;
        _depositoRepo  = depositoRepo;
    }

    // ════════════════════════════════════════════════════════════════════════
    //  BUSCAR PRODUCTO
    //  GET  → BuscarProducto?termino=X&porCodigo=true/false&idProducto=N
    // ════════════════════════════════════════════════════════════════════════

    [HttpGet]
    public async Task<IActionResult> BuscarProducto(
        string? termino    = null,
        bool    porCodigo  = false,
        int?    idProducto = null)
    {
        var vm = new BuscarProductoViewModel
        {
            Termino   = termino   ?? string.Empty,
            PorCodigo = porCodigo
        };

        // Las filas disponibles siempre se cargan para el modal Mover
        vm.FilasDisponibles = await CargarFilasDisponiblesAsync();

        try
        {
            if (idProducto.HasValue)
            {
                // Acceso directo por ID (desde VerDetalle o tabla de resultados)
                vm.ProductoDetalle = await _stockService.GetStockProductoAsync(idProducto.Value);
                return View(vm);
            }

            if (!string.IsNullOrWhiteSpace(termino))
            {
                if (porCodigo)
                {
                    vm.ProductoDetalle = await _stockService.BuscarPorCodigoAsync(termino);
                }
                else
                {
                    var resultados = await _stockService.BuscarPorNombreAsync(termino);
                    if (resultados.Count == 1)
                        vm.ProductoDetalle = await _stockService
                            .GetStockProductoAsync(resultados[0].IdProducto);
                    else
                        vm.ResultadosList = resultados;
                }
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = $"Error al buscar: {ex.Message}";
        }

        return View(vm);
    }

    // ════════════════════════════════════════════════════════════════════════
    //  PROCESAR MOVIMIENTOS (desde modal Mover Stock)
    //  POST → ProcesarMovimientos
    // ════════════════════════════════════════════════════════════════════════

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcesarMovimientos(ProcesarMovimientosViewModel vm)
    {
        if (vm.Movimientos is null || !vm.Movimientos.Any())
        {
            TempData["error"] = "No hay movimientos válidos para procesar.";
            return RedirigirPost(vm.ReturnUrl, vm.IdProducto);
        }

        int idUsuario;
        try
        {
            idUsuario = ObtenerIdUsuarioActual();
        }
        catch (Exception ex)
        {
            TempData["error"] = ex.Message;
            return RedirigirPost(vm.ReturnUrl, vm.IdProducto);
        }

        int exitosos = 0;
        var errores  = new List<string>();

        foreach (var mov in vm.Movimientos)
        {
            if (mov.Cantidad <= 0)
                { errores.Add($"Movimiento ignorado: la cantidad debe ser mayor a 0."); continue; }
            if (mov.IdFilaOrigen == mov.IdFilaDestino)
                { errores.Add("Movimiento ignorado: origen y destino son la misma fila."); continue; }
            if (mov.IdFilaOrigen <= 0 || mov.IdFilaDestino <= 0)
                { errores.Add("Movimiento ignorado: fila inválida."); continue; }

                var movVm = new MoverStockViewModel
                {
                    IdProducto    = vm.IdProducto,
                    IdFilaOrigen  = mov.IdFilaOrigen,
                    IdFilaDestino = mov.IdFilaDestino,
                    Cantidad      = mov.Cantidad,
                    Observacion   = mov.Observacion
                };

                var (ok, msg) = await _stockService.MoverStockAsync(movVm, idUsuario);
                if (ok) exitosos++;
                else    errores.Add(msg);
        }

        if (exitosos > 0 && errores.Count == 0)
            TempData["realizado"] = $"{exitosos} movimiento(s) realizados correctamente.";
        else if (exitosos > 0)
            TempData["realizado"] = $"{exitosos} OK. Advertencias: {string.Join("; ", errores)}";
        else
            TempData["error"] = errores.Count > 0
                ? string.Join("; ", errores)
                : "No se procesó ningún movimiento.";

        return RedirigirPost(vm.ReturnUrl, vm.IdProducto);
    }

    // ════════════════════════════════════════════════════════════════════════
    //  MOVER STOCK (formulario clásico — por compatibilidad)
    // ════════════════════════════════════════════════════════════════════════

    [HttpGet]
    public async Task<IActionResult> MoverStock(int idProducto, int idFilaOrigen)
    {
        try
        {
            var vm = await _stockService.GetMoverStockFormAsync(idProducto, idFilaOrigen);
            if (vm is null)
            {
                TempData["error"] = "No se encontró el producto o la fila de origen.";
                return RedirectToAction(nameof(BuscarProducto));
            }
            return View(vm);
        }
        catch (Exception)
        {
            TempData["error"] = "Error al preparar el formulario de movimiento.";
            return RedirectToAction(nameof(BuscarProducto));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MoverStock(MoverStockViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            try
            {
                var fresco = await _stockService.GetMoverStockFormAsync(vm.IdProducto, vm.IdFilaOrigen);
                if (fresco is not null)
                {
                    vm.UbicacionesActuales = fresco.UbicacionesActuales;
                    vm.FilasDestino        = fresco.FilasDestino;
                }
            }
            catch { /* no critical */ }
            return View(vm);
        }

        try
        {
            var (success, message) = await _stockService.MoverStockAsync(vm, ObtenerIdUsuarioActual());

            if (success)
            {
                TempData["realizado"] = message;
                return RedirectToAction(nameof(BuscarProducto),
                    new { termino = vm.NombreProducto });
            }

            ModelState.AddModelError(string.Empty, message);
            var fresco = await _stockService.GetMoverStockFormAsync(vm.IdProducto, vm.IdFilaOrigen);
            if (fresco is not null)
            {
                vm.UbicacionesActuales = fresco.UbicacionesActuales;
                vm.FilasDestino        = fresco.FilasDestino;
            }
            return View(vm);
        }
        catch (Exception)
        {
            TempData["error"] = "Error inesperado al mover el stock.";
            return RedirectToAction(nameof(BuscarProducto));
        }
    }

    // ════════════════════════════════════════════════════════════════════════
    //  ADMINISTRAR STOCK DE DEPÓSITO
    // ════════════════════════════════════════════════════════════════════════

    [HttpGet]
    public async Task<IActionResult> AdministrarStockDeposito(int idDeposito)
    {
        try
        {
            var vm = await _stockService.GetStockDepositoAsync(idDeposito);
            if (vm is null)
            {
                TempData["error"] = "No se encontró el depósito solicitado.";
                return RedirectToAction("Index", "Deposito");
            }
            return View(vm);
        }
        catch (Exception)
        {
            TempData["error"] = "Error al cargar el stock del depósito.";
            return RedirectToAction("Index", "Deposito");
        }
    }

    // ════════════════════════════════════════════════════════════════════════
    //  AGREGAR / RETIRAR STOCK
    // ════════════════════════════════════════════════════════════════════════

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AgregarStock(
        int idProducto, int idFila, decimal cantidad, int? idDepositoRetorno)
    {
        try
        {
            var (ok, msg) = await _stockService.AgregarStockAsync(
                idProducto, idFila, cantidad, ObtenerIdUsuarioActual());
            TempData[ok ? "realizado" : "error"] = msg;
        }
        catch (Exception) { TempData["error"] = "Error inesperado al agregar stock."; }

        return idDepositoRetorno.HasValue
            ? RedirectToAction(nameof(AdministrarStockDeposito), new { idDeposito = idDepositoRetorno.Value })
            : RedirectToAction(nameof(BuscarProducto));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RetirarStock(
        int idProducto, int idFila, decimal cantidad,
        int? idDepositoRetorno, string? returnUrl = null)
    {
        try
        {
            var (ok, msg) = await _stockService.RetirarStockAsync(
                idProducto, idFila, cantidad, ObtenerIdUsuarioActual());
            TempData[ok ? "realizado" : "error"] = msg;
        }
        catch (Exception) { TempData["error"] = "Error inesperado al retirar stock."; }

        if (!string.IsNullOrEmpty(returnUrl)) return Redirect(returnUrl);
        return idDepositoRetorno.HasValue
            ? RedirectToAction(nameof(AdministrarStockDeposito),
                new { idDeposito = idDepositoRetorno.Value })
            : RedirectToAction(nameof(BuscarProducto));
    }

    // ════════════════════════════════════════════════════════════════════════
    //  HISTORIAL
    // ════════════════════════════════════════════════════════════════════════

    [HttpGet]
    public async Task<IActionResult> HistorialMovimientos(int? idDeposito = null)
    {
        try
        {
            string? nombreDeposito = null;
            if (idDeposito.HasValue)
            {
                // Reutilizamos el depositoRepo si lo tenés inyectado,
                // o simplemente lo mostramos desde ViewBag vacío.
                // Si no tenés IDepositoRepository en este controller, inyectalo igual
                // que ya tenés _filaRepo.
                var dep = await _depositoRepo.GetByIdAsync(idDeposito.Value);
                nombreDeposito = dep?.Nombre;
            }

            var movimientos = await _stockService.GetHistorialAsync(idDeposito: idDeposito);
            ViewBag.IdDeposito      = idDeposito;
            ViewBag.NombreDeposito  = nombreDeposito ?? string.Empty;
            return View(movimientos);
        }
        catch (Exception)
        {
            TempData["error"] = "Error al cargar el historial de movimientos.";
            return RedirectToAction("Index", "Deposito");
        }
    }

    // ════════════════════════════════════════════════════════════════════════
    //  HELPERS PRIVADOS
    // ════════════════════════════════════════════════════════════════════════

    /// Carga todas las filas activas para el select de destino en el modal Mover.
    /// Usa GetAllAsync (que ya existe) y filtra por Activo.
    private async Task<List<FilaSelectItemViewModel>> CargarFilasDisponiblesAsync()
    {
        try
        {
            var filas = await _filaRepo.GetAllAsync();
            return filas
                .Where(f => f.Activo && f.Estante.Activo && f.Estante.Deposito.Activo && f.TieneEspacio && f.Estante.TieneEspacio)
                .Select(f => new FilaSelectItemViewModel
                {
                    IdFila         = f.IdFila,
                    NFila          = f.NFila,
                    IdEstante      = f.Estante.IdEstante,
                    NumeroEstante  = f.Estante.NumeroEstante,
                    IdDeposito     = f.Estante.Deposito.IdDeposito,
                    NombreDeposito = f.Estante.Deposito.Nombre
                }).ToList();
        }
        catch
        {
            return [];
        }
    }

    private IActionResult RedirigirPost(string? returnUrl, int idProducto)
    {
        if (!string.IsNullOrEmpty(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction(nameof(BuscarProducto), new { idProducto });
    }

    private int ObtenerIdUsuarioActual()
    {
        var id = _layoutService.ObtenerIdUsuario();
        if (id is null)
            throw new InvalidOperationException(
                "No se pudo identificar al usuario de la sesión. Volvé a iniciar sesión.");
        return id.Value;
    }
}