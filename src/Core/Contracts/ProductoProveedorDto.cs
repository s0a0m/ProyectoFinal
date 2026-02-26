using src.Models.Domain;
namespace src.Contracts;

public class ProductoProveedorDto
{
    public ProductoProveedor ProductoProveedor{get;set;} = new();
    public List<string> CodigosBarrasExternos { get; set; } = new();
}