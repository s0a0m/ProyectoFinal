namespace src.Presentation.ViewModels.Comprobantes;

public class GestionNotasViewModel
{
    // Datos del proveedor
    public int IdProveedor { get; set; }
    public string RazonSocial { get; set; } = string.Empty;
    public string Cuit { get; set; } = string.Empty;
    public decimal SaldoProveedor { get; set; }

    // Facturas del proveedor con sus comprobantes
    public List<FacturaConNotasVM> Facturas { get; set; } = new();

    // Motivos disponibles para NC y ND
    public List<MotivoVM> MotivosNC { get; set; } = new();
    public List<MotivoVM> MotivosND { get; set; } = new();
}

public class FacturaConNotasVM
{
    public int IdFactura { get; set; }
    public string NumeroFactura { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public decimal TotalFacturado { get; set; }
    public decimal Saldo { get; set; }
    public bool Pagada { get; set; }
    public List<ListarComprobanteViewModel> Comprobantes { get; set; } = new();
}

public class MotivoVM
{
    public short IdMotivo { get; set; }
    public string Descripcion { get; set; } = string.Empty;
}
