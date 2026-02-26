using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using src.Contracts;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace src.Presentation.ViewModels.ImportacionVM
{
    // 1. ViewModel para el Formulario de Carga (INPUT)
    public class ConfigurarCargaViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un proveedor.")]
        public short IdProveedor { get; set; }
        public IEnumerable<SelectListItem>? ProveedoresDisponibles { get; set; }
        [Required(ErrorMessage = "Debe seleccionar un archivo Excel.")]
        [Display(Name = "Archivo Excel (.xlsx)")]
        public IFormFile ArchivoExcel { get; set; }

        [Display(Name = "¿La primera fila es encabezado?")]
        public bool ContieneEncabezado { get; set; } = false;

        // Mapeo de columnas (Indices, por defecto -1 o sugeridos)
        // El usuario ingresará letras (A, B, C) o números (1, 2, 3) según decidas en la vista.
        // Aquí asumo que la vista enviará el índice numérico (A=0, B=1).

        [Display(Name = "Columna Código de Barras")]
        public int ColumnaCodigoBarra { get; set; }

        [Display(Name = "Columna Nombre/Descripción")]
        public int ColumnaNombre { get; set; } = -1;

        [Display(Name = "Columna Precio")]
        public int ColumnaPrecio { get; set; } = -1;

        [Display(Name = "Columna Stock")]
        public int ColumnaStock { get; set; } = -1; // Opcional
    }

    // 2. ViewModel para el Resultado de la Carga (OUTPUT)
    public class ReporteCargaViewModel
    {
        public short IdProveedor { get; set; }
        public string MensajeGlobal { get; set; }
        public List<AccionDeFilaCargaAutomatica> DetalleAcciones { get; set; } = new();

        // Contadores para un resumen rápido en la vista
        public int TotalProcesados => DetalleAcciones.Count;
        public int TotalActualizados => DetalleAcciones.Count(x => x.Accion.Contains("Actualización"));
        public int TotalNovedades => DetalleAcciones.Count(x => x.Accion.Contains("Novedad"));
        public int TotalIgnorados => DetalleAcciones.Count(x => x.Accion.Contains("ignora"));
    }
}