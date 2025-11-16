using Xunit;
using System.Linq;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;
using src.Models.Mappers;

namespace src.Tests;

public class DireccionProvinciaMapperTest
{
    [Fact]
    public void DireccionMapper_ListaDominioAData_MapeaColeccionCompleta()
    {
        var provA = CrearProvinciaDom(5, "Chaco");
        var provB = CrearProvinciaDom(8, "Córdoba");

        var domDirecciones = new List<Dom.Direccion>
        {
            CrearDireccionDom(20, "Calle A", provA),
            CrearDireccionDom(21, "Calle B", provB),
            CrearDireccionDom(22, "Calle C", provA)
        };

        IEnumerable<EF.Domicilio> efDirecciones = DominioMapper.Map(domDirecciones);

        Assert.Equal(3, efDirecciones.Count());

        var ultimoDomicilio = efDirecciones.Last();

        Assert.Equal(22, ultimoDomicilio.IdDomicilio);
        Assert.Equal("Calle C", ultimoDomicilio.Calle);
        Assert.Equal(1022, ultimoDomicilio.Numero);

        Assert.Equal(5, ultimoDomicilio.IdProvincia);
        Assert.NotNull(ultimoDomicilio.IdProvinciaNavigation);
        Assert.Equal("Chaco", ultimoDomicilio.IdProvinciaNavigation.Nombre);
    }
    [Fact]
    public void DireccionMapper_ListaDataADominio_MapeaExitosamente()
    {
        var prov1 = CrearProvinciaEF(1, "Salta");
        var prov2 = CrearProvinciaEF(2, "Jujuy");

        var efDirecciones = new List<EF.Domicilio>
        {
            CrearDireccionEF(10, "Calle Uno", 1, prov1),
            CrearDireccionEF(11, "Avenida Dos", 2, prov2),
            CrearDireccionEF(12, "Pasaje Tres", 1, prov1)
        };

        IEnumerable<Dom.Direccion> domDirecciones = DominioMapper.Map(efDirecciones);

        Assert.Equal(3, domDirecciones.Count());

        var ultimaDireccion = domDirecciones.Last();

        Assert.Equal(12, ultimaDireccion.IdDomicilio);
        Assert.Equal("Pasaje Tres", ultimaDireccion.Calle);
        Assert.Equal(112, ultimaDireccion.Numero);

        Assert.NotNull(ultimaDireccion.Prov);
        Assert.Equal(1, ultimaDireccion.Prov.IdProvincia);
        Assert.Equal("Salta", ultimaDireccion.Prov.Nombre);
    }
    private EF.Provincia CrearProvinciaEF(short id, string nombre)
    {
        return new EF.Provincia()
        {
            IdProvincia = id,
            Nombre = nombre
        };
    }
    private EF.Domicilio CrearDireccionEF(short id, string calle, short idProvincia, EF.Provincia prov)
    {
        return new EF.Domicilio()
        {
            IdDomicilio = id,
            IdProvincia = idProvincia,
            Calle = calle,
            Numero = (short)(100 + id),
            Piso = (short)((id % 2 == 0) ? 5 : 2),
            Comentario = $"Comentario {id}",
            IdProvinciaNavigation = prov
        };
    }
    private Dom.Provincia CrearProvinciaDom(short id, string nombre)
    {
        return new Dom.Provincia()
        {
            IdProvincia = id,
            Nombre = nombre
        };
    }

    private Dom.Direccion CrearDireccionDom(short id, string calle, Dom.Provincia prov)
    {
        return new Dom.Direccion()
        {
            IdDomicilio = id,
            Calle = calle,
            Numero = (short)(1000 + id),
            Piso = (short)(id % 2),
            Comentario = $"Comentario DOM {id}",
            Prov = prov
        };
    }
}