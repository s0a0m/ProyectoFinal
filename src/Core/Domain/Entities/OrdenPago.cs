namespace src.Models.Domain;

public class OrdenPago
{
    public int IdOrdenPago { get; set; }
    public short IdProveedor { get; set; }
    public decimal MontoTotal { get; set; }
    public string Numero { get; set; } = string.Empty;
    public DateTime? FechaPago { get; set; }
    public bool Enviada { get; set; } = false;
    public virtual Proveedor? Proveedor { get; set; }
    public virtual ICollection<PagoDetalle> Detalles { get; set; } = new List<PagoDetalle>();
}
