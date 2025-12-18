using src.Models.Common;

namespace src.Models.Domain;

public class Compra
{
    public int IdCompra { get; set; }
    public DateTime FechaCompra { get; set; }
    public string? Observaciones { get; set; }
    public DateTime? FechaRecepcion { get; set; }
    public EstadoCompra Estado { get; set; } = EstadoCompra.PENDIENTE;
    public List<DetalleCompra> Detalles { get; set; } = new();
    public decimal TotalOrden => Detalles.Sum(i => i.Subtotal);
    public Proveedor Proveedor { get; set; } = new();
    public Usuario Usuario { get; set; } = new();
}