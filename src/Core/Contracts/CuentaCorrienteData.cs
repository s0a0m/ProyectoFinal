using Dom = src.Models.Domain;

namespace src.Core.Contracts;

public class CuentaCorrienteData
{
    public Dom.Proveedor Proveedor { get; set; }
    public List<Dom.Factura> Facturas { get; set; } = new();
    public List<Dom.OrdenPago> OrdenesPago { get; set; } = new();
    public List<Dom.Comprobante> Comprobantes { get; set; } = new();
    public List<Dom.MotivoComprobante> MotivosNC { get; set; } = new();
    public List<Dom.MotivoComprobante> MotivosND { get; set; } = new();
}
