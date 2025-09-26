namespace src.Models;
public class Direccion
{
    private short _id;
    private short _id_provincia;
    private string _calle;
    private short _numero;
    private short? _piso;
    private string? _comentario;


    public short id { get => _id; set => _id = value; }
    public short id_provincia { get => _id_provincia; set => _id_provincia = value; }
    public string calle { get => _calle; set => _calle = value; }
    public short numero { get => _numero; set => _numero = value; }
    public short? piso { get => _piso; set => _piso = value; }
    public string? comentario { get => _comentario; set => _comentario = value; }
}