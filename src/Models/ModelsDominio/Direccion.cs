namespace src.Models.Domain;

public class Direccion
{
    public short IdDomicilio { get; set; }
    public Provincia Prov { get; set; } 
    public string Calle { get; set; } = string.Empty;
    public short Numero { get; set; }
    public short? Piso { get; set; }
    public string? Comentario { get; set; } = string.Empty;
}