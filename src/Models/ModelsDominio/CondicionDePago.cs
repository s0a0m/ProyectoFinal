namespace src.Models;

public abstract class CondicionDePago
{
    private short _id;
    private short _dias_pago;
    public short id { get => _id; set => _id = value; }
    public short dias_pago { get => _dias_pago; set => _dias_pago = value; }


    private string _tipo; 
    public string Tipo { get => _tipo; set => _tipo = value; }
}