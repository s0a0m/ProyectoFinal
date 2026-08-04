using src.Models.Common;
namespace src.Contracts;

public class AccionDeFilaCargaAutomatica
{
    public string Accion { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string CodigoBarra { get; set; } = string.Empty;
    public bool EsGs1 { get; set; }
}