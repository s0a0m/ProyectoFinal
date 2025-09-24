namespace src.Models;

public class Cuota : CondicionDePago
{
    private short _numeroCuotas;
    public short NumeroCuotas { get => _numeroCuotas; set => _numeroCuotas = value; }
    private float _interes_porcentual;
    public float Interes_porcentual { get => _interes_porcentual; set => _interes_porcentual = value; }

}