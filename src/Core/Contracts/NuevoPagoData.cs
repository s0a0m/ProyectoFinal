using Dom = src.Models.Domain;

namespace src.Core.Contracts;

public class NuevoPagoData
{
    public short IdProveedor { get; set; }
    public string RazonSocial { get; set; } = string.Empty;
    public List<Dom.Factura> FacturasPendientes { get; set; } = new();
}
