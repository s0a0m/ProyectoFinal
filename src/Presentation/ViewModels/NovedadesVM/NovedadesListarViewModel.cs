using System.ComponentModel.DataAnnotations;

namespace src.Presentation.ViewModels.NovedadesVM
{
    public class NovedadesListarViewModel
    {
        public int IdNovedad { get; set; }

        [Display(Name = "Proveedor")]
        public string RazonSocialProveedor { get; set; }

        [Display(Name = "Código")]
        public string CodigoBarraExterno { get; set; }

        // --- DATOS DEL EXCEL (LO NUEVO) ---
        [Display(Name = "Nombre Sugerido")]
        public string NombreSugerido { get; set; }

        [Display(Name = "Precio Nuevo")]
        [DataType(DataType.Currency)]
        public decimal PrecioSugerido { get; set; }

        [Display(Name = "Stock Excel")]
        public int StockSugerido { get; set; }

        // --- FALTABA: CONTEXTO DEL SISTEMA (PARA COMPARAR) ---

        public int? IdProductoExistente { get; set; } // Para saber si es ALTA o MODIFICACION

        [Display(Name = "Producto Actual")]
        public string? NombreProductoActual { get; set; } // Para ver si el nombre cambió mucho

        [Display(Name = "Precio Actual")]
        [DataType(DataType.Currency)]
        public decimal? PrecioActualSistema { get; set; } // Vital para ver el % de aumento

        // --- FALTABA: AUDITORÍA Y ESTADO ---

        [Display(Name = "Estado")]
        public string Estado { get; set; } // "PENDIENTE", "ACEPTADO", etc.

        [Display(Name = "Fecha Importación")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime FechaImportacion { get; set; }

        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; } // Ej: "Actualizado por re-carga"

        // --- PROPIEDADES CALCULADAS PARA LA VISTA (AYUDA VISUAL) ---

        // Te dice si es "Producto Nuevo" o "Actualización de Precio"
        public string TipoNovedad => IdProductoExistente.HasValue ? "Actualización" : "Alta Nueva";

        // Calcula el porcentaje de aumento automáticamente
        public string VariacionPrecio
        {
            get
            {
                if (!PrecioActualSistema.HasValue || PrecioActualSistema.Value == 0) return "-";

                var variacion = ((PrecioSugerido - PrecioActualSistema.Value) / PrecioActualSistema.Value) * 100;
                return $"{variacion:N2}%"; // Ej: "15.50%"
            }
        }
    }
}