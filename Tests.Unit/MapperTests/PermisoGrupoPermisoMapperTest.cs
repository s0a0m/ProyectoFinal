using Xunit;
using System.Linq;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;
using src.Models.Mappers;

namespace src.Tests;

public class PermisoGrupoPermisoMapperTest
{
    [Fact]
    public void GrupoPermisosMapper_ListaDataADominio_MapeaColeccionMMyFuncionalidad()
    {
        var efPermisos = new List<EF.Permiso>
        {
            CrearPermisoEF(1, "Leer"),
            CrearPermisoEF(2, "Escribir"),
            CrearPermisoEF(3, "Eliminar")
        };

        var efGrupos = new List<EF.GrupoPermisos>
        {
            CrearGrupoPermisosEF(10, "Administradores", efPermisos),
            CrearGrupoPermisosEF(11, "Lectores", efPermisos.Take(1))
        };

        IEnumerable<Dom.GrupoPermisos> domGrupos = DominioMapper.Map(efGrupos);

        Assert.Equal(2, domGrupos.Count());

        var admins = domGrupos.First();
        Assert.Equal("Administradores", admins.Nombre);
        Assert.Equal(3, admins.Permisos.Count);

        Assert.True(admins.HasPermiso("Escribir"), "El grupo Admin debe tener permiso Escribir.");

        var lectores = domGrupos.Last();
        Assert.Equal(1, lectores.Permisos.Count);
        Assert.False(lectores.HasPermiso("Eliminar"), "El grupo Lector no debe tener permiso Eliminar.");
    }
    [Fact]
    public void GrupoPermisosMapper_ListaDominioAData_MapeaCamposCore()
    {
        var domPermisos = new List<Dom.Permiso>
    {
        CrearPermisoDom(4, "Crear_Orden"),
        CrearPermisoDom(5, "Ver_Caja")
    };
        var domGrupos = new List<Dom.GrupoPermisos>
    {
        CrearGrupoPermisosDom(20, "Ventas", domPermisos),
        CrearGrupoPermisosDom(21, "Gerencia", domPermisos.Skip(1))
    };

        IEnumerable<EF.GrupoPermisos> efGrupos = DominioMapper.Map(domGrupos);

        Assert.Equal(2, efGrupos.Count());

        var efVentas = efGrupos.First();
        Assert.Equal(20, efVentas.IdGrupoPermiso);
        Assert.Equal("Ventas", efVentas.Nombre);

        Assert.Empty(efVentas.GruposPermisosPermisos);
        Assert.Empty(efVentas.UsuariosGruposPermisos);
    }

    private Dom.Permiso CrearPermisoDom(int id, string nombre)
    {
        return new Dom.Permiso { IdPermiso = id, Nombre = nombre, Descripcion = $"Desc {nombre}" };
    }
    private EF.Permiso CrearPermisoEF(int id, string nombre)
    {
        return new EF.Permiso { IdPermiso = id, Nombre = nombre, Descripcion = $"Desc {nombre}" };
    }
    private Dom.GrupoPermisos CrearGrupoPermisosDom(short id, string nombre, IEnumerable<Dom.Permiso> permisos)
    {
        return new Dom.GrupoPermisos(id, nombre, $"Desc {nombre}", permisos);
    }
    private EF.GrupoPermisos CrearGrupoPermisosEF(short id, string nombre, IEnumerable<EF.Permiso> permisos)
    {
        var efGrupo = new EF.GrupoPermisos
        {
            IdGrupoPermiso = id,
            Nombre = nombre,
            Descripcion = $"Desc {nombre}",
            GruposPermisosPermisos = permisos.Select(p => new EF.GrupoPermisoPermiso { Permiso = p, IdPermiso = p.IdPermiso, IdGrupoPermiso = id }).ToList()
        };
        return efGrupo;
    }
}