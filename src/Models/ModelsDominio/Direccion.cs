namespace src.Models;
public class Direccion
{
    private short _id;
    private short _id_provincia;
    private string _calle = string.Empty;
    private short _numero;
    private short? _piso;
    private string? _comentario;


    public short Id { get => _id; set => _id = value; }
    public short Id_provincia { get => _id_provincia; set => _id_provincia = value; }
    public string Calle { get => _calle; set => _calle = value; }
    public short Numero { get => _numero; set => _numero = value; }
    public short? Piso { get => _piso; set => _piso = value; }
    public string? Comentario { get => _comentario; set => _comentario = value; }
}