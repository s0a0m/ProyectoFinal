using Riok.Mapperly.Abstractions;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;

namespace src.Models.Mappers;

public static partial class DominioMapper
{
    [MapProperty(nameof(EF.MotivoComprobante.IdMotivoComprobante), nameof(Dom.MotivoComprobante.IdMotivo))]
    public static partial Dom.MotivoComprobante Map(EF.MotivoComprobante source);
    public static partial IEnumerable<Dom.MotivoComprobante> Map(IEnumerable<EF.MotivoComprobante> source);
    [MapProperty(nameof(Dom.MotivoComprobante.IdMotivo), nameof(EF.MotivoComprobante.IdMotivoComprobante))]
    public static partial EF.MotivoComprobante Map(Dom.MotivoComprobante source);
    public static partial IEnumerable<EF.MotivoComprobante> Map(IEnumerable<Dom.MotivoComprobante> source);

}