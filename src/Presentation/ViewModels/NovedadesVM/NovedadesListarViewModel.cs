using System.ComponentModel.DataAnnotations;

namespace src.Presentation.ViewModels.NovedadesVM
{
    public class NovedadesListarViewModel
    {
        public int IdNovedad { get; set; }
        // public short IdProveedor { get; set; }
        public string RazonSocialProveedor { get; set; }
        public string CodigoBarraExterno { get; set; }
        public string NombreSugerido { get; set; }
        public decimal PrecioSugerido { get; set; }
    }
}