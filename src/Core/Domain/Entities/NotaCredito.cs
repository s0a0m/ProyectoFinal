namespace src.Models.Domain;

public class NotaCredito : Comprobante
{
    public override void Aplicar(Factura factura, Proveedor proveedor)
    {
        factura.Saldo = factura.Saldo - Total;
        factura.Pagada = factura.Saldo <= 0;
        proveedor.ReducirSaldo(Total);
    }
}
