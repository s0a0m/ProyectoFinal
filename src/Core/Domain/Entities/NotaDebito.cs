namespace src.Models.Domain;

public class NotaDebito : Comprobante
{
    public override void Aplicar(Factura factura, Proveedor proveedor)
    {
        factura.Saldo += Total;
        factura.Pagada = false;
        proveedor.AumentarSaldo(Total);
    }
}
