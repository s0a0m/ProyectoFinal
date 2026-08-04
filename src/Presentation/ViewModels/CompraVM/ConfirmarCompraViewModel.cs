using System.ComponentModel.DataAnnotations;
using src.Presentation.ViewModels.CarritoVM;

namespace src.Presentation.ViewModels.CompraVM
{
    public class ConfirmarCompraViewModel
    {
        // Datos del Encabezado (Input del usuario)
        [Required]
        public short IdProveedor { get; set; }
        public string NombreProveedor { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaCompra { get; set; } = DateTime.Now;

        [Display(Name = "Observaciones / Notas")]
        public string? Observaciones { get; set; }

        // Datos de los Productos (Solo lectura para mostrar la tabla)
        public List<CarritoItemViewModel> Items { get; set; } = new();

        public decimal TotalOrden => Items.Sum(x => x.Subtotal);
    }
}