namespace src.Models;
public abstract class MetodoDePago
{
    private short _id;
    private short _dias_pago;
    public short Id { get => _id; set => _id = value; }
    public short Dias_pago { get => _dias_pago; set => _dias_pago = value; }
}