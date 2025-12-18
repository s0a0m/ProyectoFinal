using src.Models.Common;
namespace src.Contracts;

public class NovedadPendiente
{
    // si se necesitas mas campos como nombre de proveedor modiifcar aqui y en el mapper
    public int IdNovedad { get; set; }
    public short IdProducto{get;set;}
    public short IdProveedor { get; set; }
    public string CodigoBarraExterno { get; set; }
    public string NombreSugerido { get; set; }
    public decimal PrecioSugerido { get; set; }
    public EstadoNovedad Estado { get; set; }
}