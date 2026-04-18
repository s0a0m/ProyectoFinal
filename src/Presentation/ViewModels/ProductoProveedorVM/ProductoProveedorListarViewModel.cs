using System.ComponentModel.DataAnnotations;

namespace src.Presentation.ViewModels.ProductoVM;

public class SelectListItemDto
{
    public int Id { get; set; }
    public string Descripcion { get; set; } = string.Empty;
}

public class ProductoProveedorListarViewModel
{
    public int IdProducto { get; set; }
    public int IdProveedor { get; set; }

    [Display(Name = "Producto")]
    public string NombreProducto { get; set; } = string.Empty;

    [Display(Name = "Proveedor")]
    public string NombreProveedor { get; set; } = string.Empty;
    public decimal Precio { get; set; }

    // public int StockAsignado { get; set; }
    [Display(Name = "Cód. Barra Prov.")]
    public List<string> CodigosBarraExternos { get; set; } = new();
    public bool Activo { get; set; }
}

