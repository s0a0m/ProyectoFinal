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

        [MaxLength(500, ErrorMessage = "Las observaciones no pueden superar los 500 caracteres")]
        [Display(Name = "Observaciones / Notas")]
        public string? Observaciones { get; set; }

        // Datos de los Productos (Solo lectura para mostrar la tabla)
        public List<CarritoItemViewModel> Items { get; set; } = new();

        public decimal TotalOrden => Items.Sum(x => x.Subtotal);
        public int TotalItemsDiferentes => Items.Count;
        public int CantidadTotalUnidades => Items.Sum(x => x.Cantidad);
    }
}
