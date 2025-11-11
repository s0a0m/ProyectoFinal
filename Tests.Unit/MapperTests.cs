using Xunit;
using System.Linq;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;
using src.Models.Mappers;

namespace src.Tests;

public class MapperTests
{
    // --- 1. Pruebas de Domain -> EF (Objetos a FKs) ---

    [Fact]
    public void Map_DomProductoToEFProducto_MapsFKsCorrectly()
    {
        // Arrange
        // Crear las entidades de Dominio con IDs
        var domGrupo = new Dom.Grupo(idGrupo: 5, nombre: "Test", descripcion: "");
        var domCategoria = new Dom.Categoria(idCategoria: 10, nombre: "Test", descripcion: "");


        // Crear la entidad principal de Dominio
        var domProducto = new Dom.Producto(
            idProducto: 1,
            nombre: "Laptop",
            stockMinimo: 5,
            stockTotal: 50,
            activo: true,
            grupo: domGrupo,
            categoria: domCategoria
        );

        var domCodigoBarra = new Dom.CodigoBarra(idCodigoBarra: 99, codigo: "ABC-123", producto: domProducto);
        domProducto.CodigoBarra = domCodigoBarra;
        // Act
        var efProducto = DominioMapper.Map(domProducto);

        // Assert
        // Verificar que el mapeo tomó los IDs de los objetos de Dominio y los puso en las FKs de EF.
        Assert.Equal(domProducto.IdProducto, efProducto.IdProducto);
        Assert.Equal(domGrupo.IdGrupo, efProducto.IdGrupo);         // FK mapeada
        Assert.Equal(domCategoria.IdCategoria, efProducto.IdCategoria); // FK mapeada
        Assert.Equal(domCodigoBarra.IdCodigoBarra, efProducto.IdCodigoBarra); // FK mapeada

        // Verificar que los objetos de navegación NO se mapearon (IgnoredTarget)
        Assert.Null(efProducto.Grupo);
        Assert.Null(efProducto.Categoria);
        Assert.Null(efProducto.CodigoBarra);
    }

    [Fact]
    public void Map_DomProductoProveedorToEFProductoProveedor_MapsFKsCorrectly()
    {
        // Arrange
        var domProducto = new Dom.Producto(1, "Prod", 1, 1, true, new Dom.Grupo(1, "", ""), new Dom.Categoria(1, "", ""));
        // Necesitas una entidad Proveedor de Dominio con un ID
        var domProveedor = new Dom.Proveedor { IdProveedor = 50 };

        // Asumiendo que el constructor de Dom.ProductoProveedor fue simplificado
        var domRelacion = new Dom.ProductoProveedor(precio: 10.5m, stockAsignado: 100)
        {
            Producto = domProducto,
            Proveedor = domProveedor
        };

        // Act
        var efRelacion = DominioMapper.Map(domRelacion);

        // Assert
        Assert.Equal(domProducto.IdProducto, efRelacion.IdProducto);
        Assert.Equal(domProveedor.IdProveedor, efRelacion.IdProveedor);
        Assert.Equal(10.5m, efRelacion.Precio);

        // Verificar que los objetos de navegación NO se mapearon
        Assert.Null(efRelacion.Producto);
        Assert.Null(efRelacion.Proveedor);
    }

    // --- 2. Pruebas de EF -> Domain (Transferencia de Datos) ---

    [Fact]
    public void Map_EFProductoToDomProducto_MapsCollectionsAndObjectsCorrectly()
    {
        // Arrange
        var efGrupo = new EF.Grupo { IdGrupo = 5, Nombre = "Electronica" };
        var efRelacion = new EF.ProductoProveedor { IdProducto = 1, IdProveedor = 10, Precio = 99.99m };
        var efCategoria = new EF.Categoria { IdCategoria = 1, Descripcion = "", Nombre = "asd" };
        var efCodigoBarra = new EF.CodigoBarra { IdCodigoBarra = 1, Codigo = "" };


        var efProducto = new EF.Producto
        {
            IdProducto = 1,
            Nombre = "Monitor",
            IdGrupo = 5,
            Grupo = efGrupo, // Objeto de navegación de EF cargado
            ProductosProveedores = new List<EF.ProductoProveedor> { efRelacion },
            Categoria = efCategoria,
            CodigoBarra = efCodigoBarra
        };


        // Act
        var domProducto = DominioMapper.Map(efProducto);

        // Assert
        // Verificar transferencia de datos simples
        Assert.Equal("Monitor", domProducto.Nombre);

        // Verificar que la navegación de objetos se mapeó
        Assert.NotNull(domProducto.Grupo);
        Assert.Equal("Electronica", domProducto.Grupo.Nombre);

        // Verificar que la colección se mapeó y tiene el elemento
        Assert.Single(domProducto.RelacionesProveedor);
        var domRelacion = domProducto.RelacionesProveedor.First();
        Assert.Equal(99.99m, domRelacion.Precio);
    }
}