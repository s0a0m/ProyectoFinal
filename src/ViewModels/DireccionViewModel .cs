using System.ComponentModel.DataAnnotations;
using src.Models;

namespace src.ViewModels
{
    public class DireccionViewModel
    {
        [Required, Range(1, 24, ErrorMessage = "Seleccione una provincia válida.")]
        public short id_provincia { get; set; }
        [Required, StringLength(100)]
        public string calle { get; set; }
        [Required, Range(1, 9999)]
        public short numero { get; set; }
        [Range(1, 200)]
        public short? piso { get; set; }
        [StringLength(1000)]
        public string? comentario { get; set; }
        public Provincia[] provincias { get; set; } = Array.Empty<Provincia>();
    }
}