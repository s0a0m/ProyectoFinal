namespace src.Presentation.ViewModels.ProductoVM;

public class ProductoListarViewModel
{
    public int IdProducto { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int StockTotal { get; set; }
    public int StockMinimo { get; set; }
    public bool Activo { get; set; }
    public string CodigosBarra { get; set; } = string.Empty;
    public string Categorias { get; set; } = string.Empty;

    // ── NUEVO ──────────────────────────────────────────────────
    public List<UbicacionResumenVM> Ubicaciones { get; set; } = new();
    // ───────────────────────────────────────────────────────────

    // Propiedad calculada — sin cambios
    public bool EnAlertaStock => StockTotal < StockMinimo;

    // ── NUEVO: clase anidada para serializar en el data-attribute ──
    public class UbicacionResumenVM
    {
        public string Deposito { get; set; } = string.Empty;
        public string Estante  { get; set; } = string.Empty;
        public string Fila     { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
    }
}