using System.ComponentModel.DataAnnotations;

namespace src.Presentation.ViewModels.FacturaVM
{
    public class ActualizarFacturaViewModel
    {
        public string NumeroFactura { get; set; } = null!;
        public DateTime FechaEmision { get; set; }
        public int IdCondicionPago { get; set; }
        public bool Pagada { get; set; }
    }
}