namespace src.Models;

public class Cuota : CondicionDePago
{
    private short _numeroCuotas;
    public short numeroCuotas { get => _numeroCuotas; set => _numeroCuotas = value; }
    private float _interes_porcentual;
    public float interes_porcentual { get => _interes_porcentual; set => _interes_porcentual = value; }


    public Cuota()
    {
        Tipo = "Cuota";
    }
}