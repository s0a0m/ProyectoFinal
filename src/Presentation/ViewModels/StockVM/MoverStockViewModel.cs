// using System.ComponentModel.DataAnnotations;
// using Microsoft.AspNetCore.Mvc.Rendering;

// namespace src.Presentation.ViewModels.StockVM;

// public class MoverStockViewModel
// {
//     [Required]
//     public int    IdProducto     { get; set; }
//     public string NombreProducto { get; set; } = string.Empty;

//     [Required(ErrorMessage = "Debe seleccionar la fila de origen.")]
//     [Display(Name = "Fila de origen")]
//     public int IdFilaOrigen  { get; set; }

//     [Required(ErrorMessage = "Debe seleccionar la fila de destino.")]
//     [Display(Name = "Fila de destino")]
//     public int IdFilaDestino { get; set; }

//     [Required]
//     [Range(0.001, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
//     [Display(Name = "Cantidad a mover")]
//     public decimal Cantidad { get; set; }

//     public IEnumerable<UbicacionStockItemViewModel> UbicacionesActuales { get; set; }
//         = Enumerable.Empty<UbicacionStockItemViewModel>();

//     public IEnumerable<SelectListItem> FilasDestino { get; set; }
//         = Enumerable.Empty<SelectListItem>();
// }