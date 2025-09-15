namespace src.Models;
public class Proveedor
{
    private int _id;
    private string _cuit = string.Empty;
    private string _razon_social = string.Empty;
    private Direccion _domicilio;


    public int Id { get => _id; set => _id = value; }
    public string Cuit { get => _cuit; set => _cuit = value; }
    public Direccion Domicilio { get => _domicilio; set => _domicilio = value; }
}