using Dom = src.Models.Domain;

namespace src.Core.Contracts;

public class HistorialPagosData
{
    public short IdProveedor { get; set; }
    public string RazonSocial { get; set; } = string.Empty;
    public List<Dom.OrdenPago> Ordenes { get; set; } = new();
}
