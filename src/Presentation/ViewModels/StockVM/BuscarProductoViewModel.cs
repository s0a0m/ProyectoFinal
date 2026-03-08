// using Microsoft.AspNetCore.Mvc.Rendering;

// namespace src.Presentation.ViewModels.StockVM;

// // ── ViewModel principal para la página BuscarProducto ─────────────────────────
// public class BuscarProductoViewModel
// {
//     // Campos de búsqueda
//     public string Termino      { get; set; } = string.Empty;
//     public bool   PorCodigo    { get; set; } = false; // false = nombre parcial, true = código exacto

//     // Un solo producto seleccionado (por código, por id, o elegido de la lista)
//     public ProductoStockDetalleViewModel? ProductoDetalle { get; set; }

//     // Múltiples resultados (solo cuando busca por nombre y hay varios)
//     public List<ProductoBusquedaItemViewModel> ResultadosList { get; set; } = new();

//     // Todos los depósitos/filas activos — para los selects del modal Mover
//     public List<FilaSelectItemViewModel> FilasDisponibles { get; set; } = new();

//     // Estado
//     public bool BusquedaRealizada  => !string.IsNullOrEmpty(Termino);
//     public bool HayUnSoloResultado => ProductoDetalle is not null;
//     public bool HayVariosResultados => ResultadosList.Count > 1;
//     public bool SinResultados      => BusquedaRealizada && !HayUnSoloResultado && !HayVariosResultados;
// }

// // ── Detalle de stock de un producto ───────────────────────────────────────────
// public class ProductoStockDetalleViewModel
// {
//     public int            IdProducto     { get; set; }
//     public string         NombreProducto { get; set; } = string.Empty;
//     public List<string>   CodigosBarras  { get; set; } = new();  // ← barcodes en lugar de ID
//     public decimal        StockTotal     { get; set; }
//     public List<UbicacionStockItemViewModel> Ubicaciones { get; set; } = new();
// }

// // ── Una ubicación específica de un producto ────────────────────────────────────
// public class UbicacionStockItemViewModel
// {
//     public int     IdUbicacion    { get; set; }
//     public int     IdFila         { get; set; }
//     public string  NFila          { get; set; } = string.Empty;
//     public int     IdEstante      { get; set; }
//     public string  NumeroEstante  { get; set; } = string.Empty;
//     public int     IdDeposito     { get; set; }
//     public string  NombreDeposito { get; set; } = string.Empty;
//     public decimal Cantidad       { get; set; }
// }

// // ── Item de la lista de resultados múltiples ───────────────────────────────────
// public class ProductoBusquedaItemViewModel
// {
//     public int     IdProducto     { get; set; }
//     public string  Nombre         { get; set; } = string.Empty;
//     public string  CodigosBarras  { get; set; } = string.Empty; // "cod1, cod2"
//     public decimal StockTotal     { get; set; }
//     public int     CantidadFilas  { get; set; }
// }

// // ── Item para el select de filas destino en el modal ──────────────────────────
// public class FilaSelectItemViewModel
// {
//     public int    IdFila          { get; set; }
//     public string NFila           { get; set; } = string.Empty;
//     public int    IdEstante       { get; set; }
//     public string NumeroEstante   { get; set; } = string.Empty;
//     public int    IdDeposito      { get; set; }
//     public string NombreDeposito  { get; set; } = string.Empty;
//     public string TextoCompleto   => $"{NombreDeposito}  ›  Estante {NumeroEstante}  ›  Fila {NFila}";
// }

// // ── ViewModel para POST de movimiento de stock ─────────────────────────────────
// public class ProcesarMovimientosViewModel
// {
//     public int IdProducto { get; set; }

//     /// Lista de movimientos individuales enviados desde el modal
//     public List<MovimientoIndividualViewModel> Movimientos { get; set; } = new();

//     /// URL a la que redirigir después del proceso (para volver al origen: BuscarProducto o VerDetalle)
//     public string ReturnUrl { get; set; } = string.Empty;
// }

// public class MovimientoIndividualViewModel
// {
//     public int     IdFilaOrigen  { get; set; }
//     public int     IdFilaDestino { get; set; }
//     public decimal Cantidad      { get; set; }
//     public string? Observacion   { get; set; }
// }

// // ── (Compatibilidad hacia atrás — para StockService.MoverStockAsync) ──────────
// public class MoverStockViewModel
// {
//     public int     IdProducto    { get; set; }
//     public int     IdFilaOrigen  { get; set; }
//     public int     IdFilaDestino { get; set; }
//     public decimal Cantidad      { get; set; }
//     public string? Observacion   { get; set; }
//     public IEnumerable<SelectListItem> FilasOrigen  { get; set; } = Enumerable.Empty<SelectListItem>();
//     public IEnumerable<SelectListItem> FilasDestino { get; set; } = Enumerable.Empty<SelectListItem>();
// }

// // ── ProductoResumenDepositoViewModel — actualizado con filas de stock ──────────
// // (para VerDetalle.cshtml — que muestra los productos de un depósito)
// public class ProductoResumenDepositoViewModel
// {
//     public int     IdProducto      { get; set; }
//     public string  NombreProducto  { get; set; } = string.Empty;
//     public decimal StockEnDeposito { get; set; }
//     public int     CantidadFilas   { get; set; }
//     // Nueva propiedad — distribución por fila dentro del depósito
//     public List<FilaStockResumenViewModel> FilasConStock { get; set; } = new();
// }

// public class FilaStockResumenViewModel
// {
//     public int     IdFila        { get; set; }
//     public string  NFila         { get; set; } = string.Empty;
//     public int     IdEstante     { get; set; }
//     public string  NumeroEstante { get; set; } = string.Empty;
//     public decimal Cantidad      { get; set; }
// }