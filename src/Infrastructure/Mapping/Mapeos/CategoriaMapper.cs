using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

public static partial class DominioMapper
{
    // ---------------------------------------------------------
    // EF (Base de Datos) -> DOMINIO
    // ---------------------------------------------------------

    [MapperIgnoreSource(nameof(EF.Categoria.ProductosCategorias))]
    [MapperIgnoreSource(nameof(EF.Categoria.Familia))] 
    [MapProperty(nameof(EF.Categoria.IdFamilia), nameof(Dom.Categoria.Familia))]
    public static partial Dom.Categoria Map(EF.Categoria source);

    public static partial IEnumerable<Dom.Categoria> Map(IEnumerable<EF.Categoria> source);


    // ---------------------------------------------------------
    // DOMINIO -> EF (Base de Datos)
    // ---------------------------------------------------------

    [MapperIgnoreTarget(nameof(EF.Categoria.ProductosCategorias))]
    [MapperIgnoreTarget(nameof(Dom.Categoria.Familia))] 
    [MapProperty(nameof(Dom.Categoria.Familia.IdFamilia), nameof(EF.Categoria.IdFamilia))]
    public static partial EF.Categoria Map(Dom.Categoria source);

    public static partial IEnumerable<EF.Categoria> Map(IEnumerable<Dom.Categoria> source);


    // ---------------------------------------------------------
    // MÉTODOS AUXILIARES (Manuales)
    // ---------------------------------------------------------

    // Este es el "truco". Mapperly usará este método automáticamente cuando
    // necesite convertir un 'short' (el ID) en una 'Dom.Familia'.
    private static Dom.Familia MapFamiliaFromId(short id)
    {
        return new Dom.Familia 
        { 
            IdFamilia = id 
            // Las demás propiedades (Nombre, Descripcion) quedan vacías, 
            // rompiendo el ciclo infinito pero conservando el ID necesario.
        };
    }
}






/*using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

public static partial class DominioMapper
{
    [MapperIgnoreSource(nameof(EF.Categoria.ProductosCategorias))]
    [MapperIgnoreSource(nameof(EF.Categoria.Familia))]
    //[MapProperty(nameof(EF.Categoria.Familia), nameof(Dom.Categoria.Familia))]
    [MapProperty(nameof(EF.Categoria.IdFamilia), nameof(Dom.Categoria.Familia.IdFamilia))]
    public static partial Dom.Categoria Map(EF.Categoria source);
    public static partial IEnumerable<Dom.Categoria> Map(IEnumerable<EF.Categoria> source);
    [MapperIgnoreTarget(nameof(EF.Categoria.ProductosCategorias))]
    [MapProperty(nameof(Dom.Categoria.Familia.IdFamilia), nameof(EF.Categoria.IdFamilia))]
    [MapperIgnoreTarget(nameof(Dom.Categoria.Familia))]
    public static partial EF.Categoria Map(Dom.Categoria source);
    public static partial IEnumerable<EF.Categoria> Map(IEnumerable<Dom.Categoria> source);
}*/