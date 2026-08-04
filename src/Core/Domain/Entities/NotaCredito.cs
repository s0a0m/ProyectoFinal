namespace src.Models.Domain;

public class NotaCredito : Comprobante
{
    public override void Aplicar(Factura factura, Proveedor proveedor)
    {
        factura.Saldo = factura.Saldo - Total;
        if (factura.Saldo <= 0)
        {
            // factura.Saldo = 0;
            factura.Pagada = true;
            factura.FechaPago = DateTime.UtcNow;
        }
        proveedor.ReducirSaldo(Total);
    }
}
