using System.ComponentModel.DataAnnotations;

namespace src.Presentation.ViewModels.ProductoVM
{
    public class ProductoListarViewModel
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public int StockTotal { get; set; }
        public int StockMinimo { get; set; }
        public string CodigosBarra { get; set; } // "779001, 779002" (Para búsqueda fácil)
        public string Categorias { get; set; } // "Herramientas, Ofertas" (Para filtros)
        public bool Activo { get; set; }

        // RF 2.7 - Alerta automática (Propiedad calculada para la Vista)
        public bool EnAlertaStock => StockTotal <= StockMinimo;
        public string EstadoStock => EnAlertaStock ? "Bajo" : "Normal";
    }
}