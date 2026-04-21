namespace src.Models.Domain;

public class Factura
{
    public int IdFactura { get; set; }
    public string Numero { get; set; } = string.Empty;
    public short IdCondicionPagoUsada { get; set; }
    public DateTime FechaEmision { get; set; }
    public DateTime FechaPago { get; set; }
    public decimal TotalFacturado { get; set; }
    public decimal Saldo { get; set; }
    public bool Pagada { get; set; }

    public List<DetalleFactura> Detalles { get; set; } = new List<DetalleFactura>();
    public Proveedor Proveedor { get; set; } = new();
    public Compra? Compra { get; set; }
    public CondicionDePago? CondicionPago { get; set; }

    // Validaciones
    public bool PuedeEmitirNC => Saldo > 0 && !Pagada;
    public bool PuedeEmitirND => !Pagada;
    public bool PuedeRecibirPago => Saldo > 0 && !Pagada;
    public bool PuedeEditarse => TotalFacturado == Saldo;

    // Operaciones
    public void AplicarPago(decimal monto)
    {
        Saldo -= monto;
        if (Saldo <= 0)
        {
            Saldo = 0;
            Pagada = true;
            FechaPago = DateTime.UtcNow;
        }
    }

    public void RecalcularSaldo(decimal nuevoTotal)
    {
        decimal montoPagado = TotalFacturado - Saldo;
        TotalFacturado = nuevoTotal;
        Saldo = nuevoTotal - montoPagado;
        Pagada = Saldo <= 0;
    }
}
