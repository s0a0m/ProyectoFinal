using System.ComponentModel.DataAnnotations;

namespace src.ViewModels
{
    public class CondicionDePagoViewModel
    {
        [Required, StringLength(50)]
        public string Tipo { get; set; }
        [Required, Range(0, 365)]
        public short DiasPago { get; set; }
        [Range(0, 100)]
        public float InteresPorcentual { get; set; }
        [Range(1, 36)]
        public short NumeroCuotas { get; set; }
    }
}