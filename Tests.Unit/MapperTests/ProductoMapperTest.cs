using Xunit;
using System.Linq;
using System.Collections.Generic;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;
using src.Models.Mappers;

// Nota: Asumimos que todas las clases de unión (ProductoGrupo, ProductoCategoria, etc.)
// y las entidades base (Grupo, Categoria) existen en ambas capas (EF y Dom).

public class ProductoMapperTests
{
    // ===================================================================
    //  FUNCIONES AUXILIARES (Mocking de Entidades y Uniones)
    // ===================================================================

    // --- MOCKS DE ENTIDADES BASE ---
    private EF.Producto CrearProductoEF(short id, string nombre) =>
        new EF.Producto { IdProducto = id, Nombre = nombre, Activo = true, StockTotal = 50 };
    private Dom.Producto CrearProductoDom(short id, string nombre) =>
        new Dom.Producto { IdProducto = id, Nombre = nombre, Activo = true, StockTotal = 50 };

    private EF.Grupo CrearGrupoEF(short id, string nombre) =>
        new EF.Grupo { IdGrupo = id, Nombre = nombre };
    private Dom.Grupo CrearGrupoDom(short id, string nombre) =>
        new Dom.Grupo { IdGrupo = id, Nombre = nombre };
    private EF.Categoria CrearCategoriaEF(short id, string nombre) =>
        new EF.Categoria { IdCategoria = id, Nombre = nombre };

    // --- MOCKS DE TABLAS DE UNIÓN M:M ---
    private EF.ProductoGrupo CrearProductoGrupoEF(short prodId, short grupoId, EF.Producto prod, EF.Grupo grupo) =>
        new EF.ProductoGrupo { IdProducto = prodId, IdGrupo = grupoId, Producto = prod, Grupo = grupo };
    private EF.ProductoCategoria CrearProductoCategoriaEF(short prodId, short catId, EF.Producto prod, EF.Categoria cat) =>
        new EF.ProductoCategoria { IdProducto = prodId, IdCategoria = catId, Producto = prod, Categoria = cat };


    // ===================================================================
    //  PRUEBA 1: MAPEO DATA A DOMINIO (Lectura con Proyección M:M)
    // ===================================================================

    [Fact]
    public void ProductoMapper_DataADominio_MapeaColeccionesMMyProyectaCorrectamente()
    {
        // ARRANGE
        var efProducto = CrearProductoEF(100, "Sierra Circular");
        var efGrupoMadera = CrearGrupoEF(1, "Madera");
        var efCategoriaHerramienta = CrearCategoriaEF(5, "Herramientas");

        // Simular las Tablas de Unión M:M cargadas en la entidad EF (el origen)
        efProducto.ProductosGrupos = new List<EF.ProductoGrupo>
        {
            CrearProductoGrupoEF(100, 1, efProducto, efGrupoMadera), // Grupo: Madera
        };
        efProducto.ProductosCategorias = new List<EF.ProductoCategoria>
        {
            CrearProductoCategoriaEF(100, 5, efProducto, efCategoriaHerramienta) // Categoría: Herramientas
        };
        // Nota: Ignoramos CodigoBarra y Proveedor para enfocarnos en Categoria/Grupo

        // ACT
        Dom.Producto domProducto = DominioMapper.Map(efProducto);

        // ASSERT
        Assert.Equal(100, domProducto.IdProducto);

        // 1. Verificación de la Proyección de Grupos (M:M -> ICollection<Dom.Grupo>)
        Assert.NotNull(domProducto.Grupo);
        Assert.Single(domProducto.Grupo);
        Assert.Equal("Madera", domProducto.Grupo.First().Nombre);

        // 2. Verificación de la Proyección de Categorías (M:M -> ICollection<Dom.Categoria>)
        Assert.NotNull(domProducto.Categoria);
        Assert.Single(domProducto.Categoria);
        Assert.Equal(5, domProducto.Categoria.First().IdCategoria);
    }

    // ===================================================================
    //  PRUEBA 2: MAPEO DOMINIO A DATA (Escritura Segura - Ignorando M:M)
    // ===================================================================

    [Fact]
    public void ProductoMapper_DominioAData_IgnoraTodasColeccionesMMyMapeaCore()
    {
        // ARRANGE
        var domProducto = CrearProductoDom(200, "Taladro Inalámbrico");
        var nuevoGrupo = CrearGrupoDom(3, "Batería");

        // 1. Crear una nueva lista (que sí tiene Add)
        var listaGrupos = new List<Dom.Grupo>
        {
            nuevoGrupo
        };

        // 2. Asignar la lista completa a la propiedad IEnumerable
        domProducto.Grupo = listaGrupos; // Esto funciona gracias al 'set;'

        // ACT
        EF.Producto efProducto = DominioMapper.Map(domProducto);

        // ASSERT
        // 1. Verificación de Campos Escalares
        Assert.Equal(200, efProducto.IdProducto);
        Assert.Equal("Taladro Inalámbrico", efProducto.Nombre);

        // 2. Verificación de Seguridad M:M (Las colecciones EF deben estar vacías)
        // Esto confirma que los [MapperIgnoreTarget] funcionaron correctamente en el mapeo Dom -> EF.
        Assert.Empty(efProducto.ProductosGrupos);
        Assert.Empty(efProducto.ProductosCategorias);
        Assert.Empty(efProducto.ProductoCodigoBarras);
        Assert.Empty(efProducto.ProductosProveedores);
    }
}