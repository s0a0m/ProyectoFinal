namespace src.Models.Domain;

public class Usuario
{
    public short IdUsuario { get; set; }
    public bool Activo { get; set; }
    public string Correo { get; set; }
    public string Telefono { get; set; }
    public string Contrasenia { get; set; }
    public string Identificacion { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public DateTime FechaAlta { get; set; }
}