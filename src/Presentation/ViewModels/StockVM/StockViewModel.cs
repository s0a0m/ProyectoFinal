using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace src.Presentation.ViewModels.StockVM;

// ════════════════════════════════════════════════════════════════════════════
//  VISTAS LEGACY (StockService existente las sigue usando)
// ════════════════════════════════════════════════════════════════════════════

public class StockPorProductoViewModel
{
    public string? TerminoBusqueda { get; set; }
    public int?    IdProducto      { get; set; }
    public string? NombreProducto  { get; set; }
    public int     StockTotal      { get; set; }

    public IEnumerable<UbicacionStockItemViewModel> Ubicaciones { get; set; }
        = Enumerable.Empty<UbicacionStockItemViewModel>();

    public IEnumerable<SelectListItem> FilasDestino { get; set; }
        = Enumerable.Empty<SelectListItem>();
}

public class AdministrarStockDepositoViewModel
{
    public int    IdDeposito     { get; set; }
    public string NombreDeposito { get; set; } = string.Empty;

    public IEnumerable<ProductoEnDepositoViewModel> Productos { get; set; }
        = Enumerable.Empty<ProductoEnDepositoViewModel>();
}

public class ProductoEnDepositoViewModel
{
    public int     IdProducto      { get; set; }
    public string  NombreProducto  { get; set; } = string.Empty;
    public decimal StockEnDeposito { get; set; }

    public IEnumerable<UbicacionStockItemViewModel> Ubicaciones { get; set; }
        = Enumerable.Empty<UbicacionStockItemViewModel>();
}

/// Ubicación de un producto en una fila.
/// Versión unificada: contiene IdUbicacion (nuevo) + RutaCompleta (legacy).
public class UbicacionStockItemViewModel
{
    public int     IdUbicacion    { get; set; }   // nuevo
    public int     IdFila         { get; set; }
    public string  NFila          { get; set; } = string.Empty;
    public int     IdEstante      { get; set; }
    public string  NumeroEstante  { get; set; } = string.Empty;
    public int     IdDeposito     { get; set; }
    public string  NombreDeposito { get; set; } = string.Empty;
    public decimal Cantidad       { get; set; }

    public string RutaCompleta =>
        $"{NombreDeposito}  ▸  Estante {NumeroEstante}  ▸  Fila {NFila}";
}

/// ViewModel unificado para mover stock.
/// Contiene propiedades del modelo viejo (NombreProducto, UbicacionesActuales)
/// y del nuevo (FilasOrigen) para no romper StockService.
public class MoverStockViewModel
{
    [Required]
    public int    IdProducto     { get; set; }
    public string NombreProducto { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe seleccionar la fila de origen.")]
    [Display(Name = "Fila de origen")]
    public int IdFilaOrigen  { get; set; }

    [Required(ErrorMessage = "Debe seleccionar la fila de destino.")]
    [Display(Name = "Fila de destino")]
    public int IdFilaDestino { get; set; }

    [Required]
    [Range(0.001, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
    [Display(Name = "Cantidad a mover")]
    public decimal Cantidad { get; set; }

    public string? Observacion { get; set; }


    // Para formularios de selección
    public IEnumerable<UbicacionStockItemViewModel> UbicacionesActuales { get; set; }
        = Enumerable.Empty<UbicacionStockItemViewModel>();

    public IEnumerable<SelectListItem> FilasOrigen  { get; set; }
        = Enumerable.Empty<SelectListItem>();

    public IEnumerable<SelectListItem> FilasDestino { get; set; }
        = Enumerable.Empty<SelectListItem>();
}

public class MovimientoStockItemViewModel
{
    public int      IdMovimientoStock { get; set; }
    public string   NombreProducto    { get; set; } = string.Empty;
    public string   FilaOrigen        { get; set; } = "—";
    public string   FilaDestino       { get; set; } = "—";
    public decimal  Cantidad          { get; set; }
    public DateTime FechaMovimiento   { get; set; }
    public string   Usuario           { get; set; } = string.Empty;

    public bool EsIngreso    { get; set; }   // IdFilaOrigen  == null en BD
    public bool EsEgreso     { get; set; }   // IdFilaDestino == null en BD
    public bool EsMovimiento => !EsIngreso && !EsEgreso;

    public string TipoMovimiento =>
        FilaOrigen == "—" ? "Entrada" :
        FilaDestino == "—" ? "Salida" : "Traslado";

    public string BadgeClass =>
        TipoMovimiento == "Entrada" ? "bg-success" :
        TipoMovimiento == "Salida"  ? "bg-danger"  : "bg-info";
}

// ════════════════════════════════════════════════════════════════════════════
//  NUEVOS — BuscarProducto + ProcesarMovimientos
// ════════════════════════════════════════════════════════════════════════════

/// ViewModel principal para Views/Stock/BuscarProducto.cshtml
public class BuscarProductoViewModel
{
    public string Termino   { get; set; } = string.Empty;
    public bool   PorCodigo { get; set; } = false;

    // Un solo producto encontrado (por código exacto, por id, o único resultado de nombre)
    public ProductoStockDetalleViewModel? ProductoDetalle { get; set; }

    // Varios resultados (búsqueda por nombre con más de un resultado)
    public List<ProductoBusquedaItemViewModel> ResultadosList { get; set; } = new();

    // Filas activas del sistema — para los selects del modal Mover
    public List<FilaSelectItemViewModel> FilasDisponibles { get; set; } = new();

    public bool BusquedaRealizada =>
    !string.IsNullOrWhiteSpace(Termino) ||
    ProductoDetalle is not null          ||
    ResultadosList  is not null && ResultadosList.Count > 0;    public bool HayUnSoloResultado  => ProductoDetalle is not null;
    public bool HayVariosResultados => ResultadosList.Count > 1;
    public bool SinResultados       => BusquedaRealizada && !HayUnSoloResultado && !HayVariosResultados;
}

/// Detalle completo de stock de un único producto
public class ProductoStockDetalleViewModel
{
    public int           IdProducto     { get; set; }
    public string        NombreProducto { get; set; } = string.Empty;
    public List<string>  CodigosBarras  { get; set; } = new();
    public decimal       StockTotal     { get; set; }
    public List<UbicacionStockItemViewModel> Ubicaciones { get; set; } = new();
}

/// Item de la lista de resultados múltiples
public class ProductoBusquedaItemViewModel
{
    public int     IdProducto    { get; set; }
    public string  Nombre        { get; set; } = string.Empty;
    public string  CodigosBarras { get; set; } = string.Empty;   // "cod1, cod2"
    public decimal StockTotal    { get; set; }
    public int     CantidadFilas { get; set; }
}

/// Item para el select de filas en el modal Mover
public class FilaSelectItemViewModel
{
    public int    IdFila         { get; set; }
    public string NFila          { get; set; } = string.Empty;
    public int    IdEstante      { get; set; }
    public string NumeroEstante  { get; set; } = string.Empty;
    public int    IdDeposito     { get; set; }
    public string NombreDeposito { get; set; } = string.Empty;
    public string TextoCompleto  => $"{NombreDeposito}  ›  Estante {NumeroEstante}  ›  Fila {NFila}";
}

/// POST: múltiples movimientos enviados desde el modal
public class ProcesarMovimientosViewModel
{
    public int    IdProducto { get; set; }
    public List<MovimientoIndividualViewModel> Movimientos { get; set; } = new();
    /// URL de retorno (VerDetalle o BuscarProducto)
    public string ReturnUrl  { get; set; } = string.Empty;
}

public class MovimientoIndividualViewModel
{
    public int     IdFilaOrigen  { get; set; }
    public int     IdFilaDestino { get; set; }
    public decimal Cantidad      { get; set; }
    public string? Observacion   { get; set; }
}

// ════════════════════════════════════════════════════════════════════════════
//  VerDetalle — distribución por fila de un producto dentro de un depósito
// ════════════════════════════════════════════════════════════════════════════

public class ProductoResumenDepositoViewModel
{
    public int     IdProducto      { get; set; }
    public string  NombreProducto  { get; set; } = string.Empty;
    public decimal StockEnDeposito { get; set; }
    public int     CantidadFilas   { get; set; }
    public List<FilaStockResumenViewModel> FilasConStock { get; set; } = new();
}

public class FilaStockResumenViewModel
{
    public int     IdFila        { get; set; }
    public string  NFila         { get; set; } = string.Empty;
    public int     IdEstante     { get; set; }
    public string  NumeroEstante { get; set; } = string.Empty;
    public decimal Cantidad      { get; set; }
}