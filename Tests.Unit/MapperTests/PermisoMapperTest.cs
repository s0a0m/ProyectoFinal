using Xunit;
using System.Linq;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;
using src.Models.Mappers;

namespace src.MapperTests;

public class PermisoMapperTest
{
    [Fact]
    public void PermisoMapper_ListaDominioAData_MapeaCamposCore()
    {
        var domPermisos = new List<Dom.Permiso>
        {
            CrearPermisoDom(id: 201, nombre: "VER_REPORTES"),
            CrearPermisoDom(id: 202, nombre: "CIERRE_CAJA"),
        };
        IEnumerable<EF.Permiso> efPermisos = DominioMapper.Map(domPermisos);

        Assert.Equal(2, efPermisos.Count());

        var primerPermiso = efPermisos.First();
        Assert.Equal(201, primerPermiso.IdPermiso);
        Assert.Equal("VER_REPORTES", primerPermiso.Nombre);

        Assert.Null(primerPermiso.UsuariosPermisos);
        Assert.Null(primerPermiso.GruposPermisosPermisos);
    }
    [Fact]
    public void PermisoMapper_ListaDataADominio_MapeaCamposYIgnoraColecciones()
    {
        var efPermisos = new List<EF.Permiso>
        {
            CrearPermisoEF(id: 101, nombre: "CREAR_PRODUCTO"),
            CrearPermisoEF(id: 102, nombre: "EDITAR_STOCK"),
        };
        IEnumerable<Dom.Permiso> domPermisos = DominioMapper.Map(efPermisos);

        Assert.Equal(2, domPermisos.Count());

        var ultimoPermiso = domPermisos.Last();
        Assert.Equal(102, ultimoPermiso.IdPermiso);
        Assert.Equal("EDITAR_STOCK", ultimoPermiso.Nombre);
        Assert.Contains("EDITAR_STOCK", ultimoPermiso.Descripcion);
    }
    private EF.Permiso CrearPermisoEF(int id, string nombre)
    {
        return new EF.Permiso
        {
            IdPermiso = id,
            Nombre = nombre,
            Descripcion = $"Descripción para {nombre}"
            // No inicializamos las colecciones de unión (UsuariosPermisos, GruposPermisosPermisos) 
            // ya que las ignoraremos en el mapeo.
        };
    }
    private Dom.Permiso CrearPermisoDom(int id, string nombre)
    {
        return new Dom.Permiso
        {
            IdPermiso = id,
            Nombre = nombre,
            Descripcion = $"Descripción para {nombre}"
        };
    }
}