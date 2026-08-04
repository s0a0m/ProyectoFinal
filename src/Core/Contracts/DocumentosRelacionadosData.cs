// Core/Contracts/DocumentosRelacionadosData.cs
using Dom = src.Models.Domain;

namespace src.Core.Contracts;

public class DocumentosRelacionadosData
{
    public Dom.Factura Factura { get; set; }
    public List<Dom.Comprobante> Comprobantes { get; set; } = new();
    public List<Dom.OrdenPago> OrdenesPago { get; set; } = new();
}
