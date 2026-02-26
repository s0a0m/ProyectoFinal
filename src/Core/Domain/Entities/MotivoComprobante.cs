namespace src.Models.Domain;

public class MotivoComprobante
{
    public short IdMotivo { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string ClaseComprobante { get; set; } = string.Empty; // "NC", "ND", "AMBOS"
}