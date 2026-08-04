// namespace src.Presentation.ViewModels.StockVM;


// public class MovimientoStockItemViewModel
// {
//     public int      IdMovimientoStock { get; set; }
//     public string   NombreProducto    { get; set; } = string.Empty;
//     public string   FilaOrigen        { get; set; } = "—";
//     public string   FilaDestino       { get; set; } = "—";
//     public decimal  Cantidad          { get; set; }
//     public DateTime FechaMovimiento   { get; set; }
//     public string   Usuario           { get; set; } = string.Empty;

//     public string TipoMovimiento =>
//         FilaOrigen == "—" ? "Entrada" :
//         FilaDestino == "—" ? "Salida" : "Traslado";

//     public string BadgeClass =>
//         TipoMovimiento == "Entrada" ? "bg-success" :
//         TipoMovimiento == "Salida"  ? "bg-danger"  : "bg-info";
// }