using Xunit;
using System.Linq;
using System.Collections.Generic;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;
using src.Models.Mappers;
namespace src.MapperTests;

public class FamiliaCategoriaMapperTests
{
    // ===================================================================
    //  FUNCIONES AUXILIARES (Mocking de Entidades)
    // ===================================================================

    private EF.Familia CrearFamiliaEF(short id, string nombre) =>
        new EF.Familia { IdFamilia = id, Nombre = nombre, Descripcion = $"Desc {nombre}" };

    private Dom.Familia CrearFamiliaDom(short id, string nombre) =>
        new Dom.Familia { IdFamilia = id, Nombre = nombre, Descripcion = $"Desc {nombre}" };

    private EF.Categoria CrearCategoriaEF(short id, string nombre, short idFamilia, EF.Familia familia) =>
        new EF.Categoria
        {
            IdCategoria = id,
            Nombre = nombre,
            Descripcion = $"Desc {nombre}",
            IdFamilia = idFamilia,
            Familia = familia, // Objeto de navegación EF
            ProductosCategorias = new List<EF.ProductoCategoria>()
        };

    private Dom.Categoria CrearCategoriaDom(short id, string nombre, Dom.Familia familia) =>
        new Dom.Categoria
        {
            IdCategoria = id,
            Nombre = nombre,
            Descripcion = $"Desc {nombre}",
            Familia = familia // Objeto de navegación Dom
        };


    // ===================================================================
    //  PRUEBA 1: MAPEO DATA A DOMINIO (Lectura con Relación Anidada)
    // ===================================================================

    [Fact]
    public void CategoriaMapper_ListaDataADominio_MapeaRelacionYCamposCore()
    {
        // ARRANGE
        var efFamilia = CrearFamiliaEF(10, "Clasificación A");

        var efCategorias = new List<EF.Categoria>
        {
            CrearCategoriaEF(1, "Taladros", 10, efFamilia),
            CrearCategoriaEF(2, "Martillos", 10, efFamilia),
        };

        // ACT
        IEnumerable<Dom.Categoria> domCategorias = DominioMapper.Map(efCategorias);

        // ASSERT
        Assert.Equal(2, domCategorias.Count());

        var cat1 = domCategorias.First();

        // 1. Verificación de campos simples
        Assert.Equal("Taladros", cat1.Nombre);

        // 2. Verificación de la Relación (El objeto Familia debe estar mapeado)
        Assert.NotNull(cat1.Familia);
        Assert.Equal(10, cat1.Familia.IdFamilia);
        Assert.Equal("Clasificación A", cat1.Familia.Nombre);
    }

    // ===================================================================
    //  PRUEBA 2: MAPEO DOMINIO A DATA (Escritura Segura - Solo FK)
    // ===================================================================

    [Fact]
    public void CategoriaMapper_ListaDominioAData_MapeaFKyIgnoraObjetoCompleto()
    {
        // ARRANGE
        var domFamilia = CrearFamiliaDom(20, "Clasificación B");

        // Crear las categorías de dominio, adjuntando el objeto Familia
        var domCategorias = new List<Dom.Categoria>
        {
            CrearCategoriaDom(3, "Tornillos", domFamilia),
            CrearCategoriaDom(4, "Clavos", domFamilia),
        };

        // ACT
        IEnumerable<EF.Categoria> efCategorias = DominioMapper.Map(domCategorias);

        // ASSERT
        Assert.Equal(2, efCategorias.Count());

        var catEF = efCategorias.First();

        // 1. Verificación de Campos Simples
        Assert.Equal(3, catEF.IdCategoria);
        Assert.Equal("Tornillos", catEF.Nombre);

        // 2. Verificación de la Persistencia (Mapea SOLO la FK, ignora el objeto)
        Assert.Equal(20, catEF.IdFamilia); // Verifica que la FK se mapeó correctamente
        Assert.Null(catEF.Familia); // Verifica que el objeto de navegación fue ignorado (MapperIgnoreSource)

        // 3. Verificación de Seguridad M:M
        Assert.Empty(catEF.ProductosCategorias); // Confirma que la colección M:M se ignoró (MapperIgnoreTarget)
    }
}