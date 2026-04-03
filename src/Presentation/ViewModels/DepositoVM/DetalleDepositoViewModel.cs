namespace src.Presentation.ViewModels.DepositoVM;

public class DetalleDepositoViewModel
{
    public int    IdDeposito      { get; set; }
    public string Nombre          { get; set; } = string.Empty;
    public string DireccionResumen{ get; set; } = string.Empty;
    public bool   Activo          { get; set; }

    public IEnumerable<EstanteDetalleViewModel> Estantes { get; set; }
        = Enumerable.Empty<EstanteDetalleViewModel>();

    /// Productos con stock en cualquier fila del depósito (vista consolidada)
    public IEnumerable<ProductoResumenDepositoViewModel> Productos { get; set; }
        = Enumerable.Empty<ProductoResumenDepositoViewModel>();


    public DepositoFormViewModel FormularioEdicion { get; set; } = new();

    // Totales rápidos para el encabezado
    public int     TotalEstantes   => Estantes.Count();
    public int     TotalFilas      => Estantes.Sum(e => e.Filas.Count());
    public decimal TotalStockUnidades => Productos.Sum(p => p.StockEnDeposito);
}

public class EstanteDetalleViewModel
{
    public int     IdEstante     { get; set; }
    public string  NumeroEstante { get; set; } = string.Empty;
    public bool    Activo        { get; set; }
    public bool    TieneEspacio  { get; set; }
    public string? Observaciones { get; set; }

    public IEnumerable<FilaDetalleViewModel> Filas { get; set; }
        = Enumerable.Empty<FilaDetalleViewModel>();

    public decimal TotalStockEstante => Filas.Sum(f => f.TotalStockFila);
    public int     TotalFilas        => Filas.Count();
}

public class FilaDetalleViewModel
{
    public int     IdFila        { get; set; }
    public string  NFila         { get; set; } = string.Empty;
    public bool    Activo        { get; set; }
    public bool    TieneEspacio  { get; set; }
    public string? Observaciones { get; set; }
    public decimal TotalStockFila{ get; set; }
}

public class ProductoResumenDepositoViewModel
{
    public int     IdProducto      { get; set; }
    public string  NombreProducto  { get; set; } = string.Empty;
    public decimal StockEnDeposito { get; set; }
    public int     CantidadFilas   { get; set; }  // en cuántas filas está distribuido
}