using System.ComponentModel.DataAnnotations;
using Dom = src.Models.Domain; // Alias para el namespace de Dominio

namespace src.ViewModels
{
    public class ActualizarProveedorViewModel
    {
        [Required] 
        public int IdProveedor { get; set; } 


        [Required(ErrorMessage = "El CUIT es obligatorio.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "El CUIT debe tener 11 dígitos numéricos.")] 
        public string Cuit { get; set; } = string.Empty;

        [Required(ErrorMessage = "La razón social es obligatoria.")]
        [StringLength(100)]
        public string RazonSocial { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de teléfono es obligatorio.")]
        [StringLength(12)]
        [Phone(ErrorMessage = "Formato de teléfono no válido.")] 
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Mail es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo no válido."), StringLength(50)]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre de la Persona responsable es obligatorio.")]
        [StringLength(80)]
        public string PersonaResponsable { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Saldo es obligatorio.")]
        [Range(0, 9999999999.99, ErrorMessage = "El saldo debe ser un valor positivo.")] // Mantener validación
        public decimal Saldo { get; set; }

      

        [Required(ErrorMessage = "La condición de pago es obligatoria.")]
        public CondicionDePagoViewModel CondicionPago { get; set; } = new();

        [Required(ErrorMessage = "La Dirección es obligatoria.")]
        public DireccionViewModel Direccion { get; set; } = new();

        // --- Constructores ---

        /// <summary>
        /// Constructor vacío necesario para el Model Binding en POST.
        /// </summary>
        public ActualizarProveedorViewModel() { }

        /// <summary>
        /// Constructor para rellenar el ViewModel desde el modelo de Dominio.
        /// Usado en el controlador [HttpGet] ActualizarProveedor.
        /// </summary>
        /// <param name="p">El objeto Dom.Proveedor obtenido de la base de datos.</param>
        /// <param name="listaProvincias">La lista completa de provincias para el dropdown.</param>
        public ActualizarProveedorViewModel(Dom.Proveedor p, List<Dom.Provincia> listaProvincias)
        {
            IdProveedor = p.IdProveedor;
            Cuit = p.Cuit;
            RazonSocial = p.RazonSocial;
            Telefono = p.Telefono;
            Correo = p.Correo;
            PersonaResponsable = p.PersonaResponsable;
            Saldo = p.Saldo;

            // Mapear Dirección
            Direccion = new DireccionViewModel
            {
                calle = p.Direccion.Calle,
                numero = p.Direccion.Numero,
                piso = p.Direccion.Piso,
                comentario = p.Direccion.Comentario,
                ListaProvincias = listaProvincias,
                provincia = new ProvinciaViewModel
                {
                    Id_provincia = p.Direccion.Prov.IdProvincia 
                }
            };

            if (p.Condicion != null)
            {
                CondicionPago = new CondicionDePagoViewModel
                {
                    DiasPago = p.Condicion.DiasPago,
                    Tipo = (p.Condicion is Dom.Cuota) ? "Cuota" : "Contado", 

                 
                    NumeroCuotas = (p.Condicion as Dom.Cuota)?.Cuotas ?? 0, 
                    InteresPorcentual = (p.Condicion as Dom.Cuota)?.InteresPorcentual ?? 0M 
                };
            }
        }
    }
}