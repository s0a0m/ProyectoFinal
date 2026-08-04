using Dom = src.Models.Domain;

namespace src.Core.Contracts;

public class GestionNotasData
{
    public Dom.Proveedor Proveedor { get; set; }
    public List<FacturaConNotas> Facturas { get; set; } = new();
    public List<Dom.MotivoComprobante> MotivosNC { get; set; } = new();
    public List<Dom.MotivoComprobante> MotivosND { get; set; } = new();
}

public class FacturaConNotas
{
    public Dom.Factura Factura { get; set; }
    public List<Dom.Comprobante> Comprobantes { get; set; } = new();
}
