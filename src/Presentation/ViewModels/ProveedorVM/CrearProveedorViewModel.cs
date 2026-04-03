using System.ComponentModel.DataAnnotations;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using src.Models.CodeFirst;
using Dom = src.Models.Domain;
namespace src.ViewModels;

public class CrearProveedorViewModel
{
    public CrearProveedorViewModel()
    {
    }

    [Required(ErrorMessage = "El CUIT es obligatorio.")]
    [StringLength(11, ErrorMessage = "El CUIT no puede exceder los 11 caracteres.")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "El CUIT debe contener exactamente 11 dígitos numéricos.")]
    public string Cuit { get; set; }

    [Required(ErrorMessage = "La razon social es obligatoria.")]
    [RegularExpression(@"^[\p{L}\p{N}\s]+$", ErrorMessage = "La razón social no puede contener caracteres especiales")]
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
    [RegularExpression(@"^[\p{L}\p{N}\s]+$", ErrorMessage = "La Persona no puede contener caracteres especiales")]
    public string PersonaResponsable { get; set; }

    [Required(ErrorMessage = "El Saldo es obligatorio.")]
    [Range(0, 99999999.99)]
    public decimal Saldo { get; set; }


    [Required(ErrorMessage = "La condicion de pago es obligatoria.")]
    public CondicionDePagoViewModel CondicionPago { get; set; } = new();


    [Required(ErrorMessage = "La Direccion es obligatoria.")]
    public DireccionViewModel Direccion { get; set; } = new();


}
