namespace src.Models.Domain;

public class MovimientoStock
{
    public int IdMovimientoStock { get; set; }
    public Producto Producto { get; set; } = new Producto();
    public Fila? FilaOrigen { get; set;}
    public Fila? FilaDestino { get; set;}
    public decimal Cantidad { get; set; }
    public DateTime FechaMovimiento { get; set; }
    public Usuario Usuario { get; set; } = new Usuario();
    
}