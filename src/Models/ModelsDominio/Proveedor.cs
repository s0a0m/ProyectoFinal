namespace src.Models.Domain;

public class Proveedor
{
  public int IdProveedor { get; set; }
  public string Cuit { get; set; } = string.Empty;
  public string Telefono { get; set; } = string.Empty;
  public string Correo { get; set; } = string.Empty;
  public string PersonaResponsable { get; set; } = string.Empty;
  public decimal Saldo { get; set; }
  public Direccion Direccion { get; set; } = new Direccion();
  public bool Activo { get; set; }
  public string RazonSocial { get; set; } = string.Empty;
  public CondicionDePago? Condicion { get; set; }

}