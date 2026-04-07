using System.ComponentModel.DataAnnotations;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using src.Models.CodeFirst;
using src.Presentation.ViewModels;
using src.Presentation.ViewModels.CondicionPagoVM;
using src.Presentation.ViewModels.DireccionVM;
using Dom = src.Models.Domain;

namespace src.Presentation.ViewModels.ProveedorVM;

public class CrearProveedorViewModel
{
    public CrearProveedorViewModel() { }

    [Required(ErrorMessage = "El CUIT es obligatorio.")]
    [StringLength(11, ErrorMessage = "El CUIT no puede exceder los 11 caracteres.")]
    [RegularExpression(
        @"^\d{11}$",
        ErrorMessage = "El CUIT debe contener exactamente 11 dígitos numéricos."
    )]
    public string Cuit { get; set; }

    [Required(ErrorMessage = "La razon social es obligatoria.")]
    [RegularExpression(
        @"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s.,&/-]+$",
        ErrorMessage = "La razón social contiene caracteres no permitidos"
    )]
    [StringLength(80)]
    public string RazonSocial { get; set; }

    [Required(ErrorMessage = "El numero de telefono es obligatorio.")]
    [StringLength(12)]
    public string Telefono { get; set; }

    [Required(ErrorMessage = "El Mail es obligatorio.")]
    [EmailAddress, StringLength(50)]
    public string Correo { get; set; }

    [Required(ErrorMessage = "El nombre de la Persona responsable es obligatorio.")]
    [StringLength(80)]
    [RegularExpression(
        @"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s.,&/-]+$",
        ErrorMessage = "El nombre de la persona contiene caracteres no permitidos"
    )]
    public string PersonaResponsable { get; set; }

    [Required(ErrorMessage = "El Saldo es obligatorio.")]
    [Range(0, 99999999.99)]
    public decimal Saldo { get; set; }

    [Required(ErrorMessage = "La condicion de pago es obligatoria.")]
    public CondicionDePagoViewModel CondicionPago { get; set; } = new();

    [Required(ErrorMessage = "La Direccion es obligatoria.")]
    public DireccionViewModel Direccion { get; set; } = new();
}
