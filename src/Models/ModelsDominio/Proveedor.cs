namespace src.Models;

public class Proveedor
{
  private int _id;
  private string _cuit = string.Empty;
  private string _razon_social = string.Empty;
  private CondicionDePago _condicion;
  private string _telefono;
  private string _correo;
  private string _personaResponsable;
  private decimal _saldo;
  private Direccion _direccion;
  private bool _activo;
  public int id { get => _id; set => _id = value; }
  public string cuit { get => _cuit; set => _cuit = value; }
  public string telefono { get => _telefono; set => _telefono = value; }
  public string correo { get => _correo; set => _correo = value; }
  public string personaResponsable { get => _personaResponsable; set => _personaResponsable = value; }
  public decimal saldo { get => _saldo; set => _saldo = value; }
  public Direccion direccion { get => _direccion; set => _direccion = value; }
  public bool activo { get => _activo; set => _activo = value; }
  public string razonSocial { get => _razon_social; set => _razon_social = value; }
  public CondicionDePago condicion { get => _condicion; set => _condicion = value; }
}