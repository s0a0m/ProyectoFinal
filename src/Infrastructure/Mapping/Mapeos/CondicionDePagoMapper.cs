using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

public static partial class DominioMapper
{
    [MapDerivedType(typeof(EF.Contado), typeof(Dom.Contado))]
    [MapDerivedType(typeof(EF.Cuota), typeof(Dom.Cuota))]
    public static partial Dom.CondicionDePago Map(EF.CondicionDePago source);
    public static partial IEnumerable<Dom.CondicionDePago> Map(IEnumerable<EF.CondicionDePago> source);

    [MapDerivedType(typeof(Dom.Contado), typeof(EF.Contado))]
    [MapDerivedType(typeof(Dom.Cuota), typeof(EF.Cuota))]
    public static partial EF.CondicionDePago Map(Dom.CondicionDePago source);
    public static partial IEnumerable<EF.CondicionDePago> Map(IEnumerable<Dom.CondicionDePago> source);
}