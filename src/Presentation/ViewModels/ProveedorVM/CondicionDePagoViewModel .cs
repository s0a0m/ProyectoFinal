using System.ComponentModel.DataAnnotations;

namespace src.ViewModels
{
    public class CondicionDePagoViewModel
    {
        public CondicionDePagoViewModel()
        {
        }

        [Required(ErrorMessage = "El Tipo de pago es obligatorio.")]
        [StringLength(50)]
        public string Tipo { get; set; }
        [Required, Range(0, 365)]
        public short DiasPago { get; set; }
        public decimal InteresPorcentual { get; set; }    
        public short NumeroCuotas { get; set; }

        
    }
}