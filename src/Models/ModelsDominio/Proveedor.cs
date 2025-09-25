namespace src.Models;

public class Proveedor
{
  private int _id;
  private string _cuit = string.Empty;
  private string _razon_social = string.Empty;
  private Direccion _domicilio;
  private CondicionDePago _condicion;
  private string _telefono;
  private string _correo;
  private string _personaResponsable;
  private decimal _saldo;
  private Direccion _direccion;

  private bool activo;
  public int Id { get => _id; set => _id = value; }
  public string Cuit { get => _cuit; set => _cuit = value; }
  public Direccion Domicilio { get => _domicilio; set => _domicilio = value; }
  public string Telefono { get => _telefono; set => _telefono = value; }
  public string Correo { get => _correo; set => _correo = value; }
  public string PersonaResponsable { get => _personaResponsable; set => _personaResponsable = value; }
  public decimal Saldo { get => _saldo; set => _saldo = value; }
  public Direccion Direccion { get => _direccion; set => _direccion = value; }
  public bool Activo { get => activo; set => activo = value; }
  public string RazonSocial { get => _razon_social; set => _razon_social = value; }
  public CondicionDePago Condicion { get => _condicion; set => _condicion = value; }
}