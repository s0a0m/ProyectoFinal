using Xunit;
using System.Linq;
using System.Collections.Generic;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;
using src.Models.Mappers;

namespace src.Tests;

public class UsuarioMapperTests
{
    private EF.Permiso CrearPermisoEF(int id, string nombre) =>
        new EF.Permiso { IdPermiso = id, Nombre = nombre };
    private EF.GrupoPermisos CrearGrupoEF(short id, string nombre) =>
        new EF.GrupoPermisos { IdGrupoPermiso = id, Nombre = nombre };
    private EF.Usuario CrearUsuarioEF(short id, string nombre, string apellido,
        ICollection<EF.UsuarioPermiso> userPerms = null, ICollection<EF.UsuarioGrupoPermisos> userGroups = null)
    {
        return new EF.Usuario
        {
            IdUsuario = id,
            Nombre = nombre,
            Apellido = apellido,
            Activo = true,
            Correo = $"{nombre}@{apellido}.com",
            Telefono = "123456789",
            Contrasenia = "hash",
            Identificacion = $"ID{id}",
            FechaAlta = new DateTime(2023, 1, 1),
            UsuariosPermisos = userPerms ?? new List<EF.UsuarioPermiso>(),
            UsuariosGruposPermisos = userGroups ?? new List<EF.UsuarioGrupoPermisos>()
        };
    }

    private Dom.Usuario CrearUsuarioDom(short id, string nombre, string apellido,
        IEnumerable<Dom.Permiso> userPerms = null, IEnumerable<Dom.GrupoPermisos> userGroups = null)
    {
        return new Dom.Usuario
        {
            IdUsuario = id,
            Nombre = nombre,
            Apellido = apellido,
            Activo = true,
            Correo = $"{nombre}@{apellido}.dom",
            Telefono = "11223344",
            Contrasenia = "dom_hash",
            Identificacion = $"DOM{id}",
            FechaAlta = new DateTime(2024, 1, 1),
            PermisosUsuario = userPerms ?? Enumerable.Empty<Dom.Permiso>(),
            GrupoPermisos = userGroups ?? Enumerable.Empty<Dom.GrupoPermisos>()
        };
    }
    [Fact]
    public void UsuarioMapper_DataADominio_MapeaColeccionesMMyDatosBase()
    {
        var efPermiso1 = CrearPermisoEF(101, "EDITAR_PERFIL");
        var efGrupo1 = CrearGrupoEF(5, "Administradores");

        var efUserPermisos = new List<EF.UsuarioPermiso>
        {
            new EF.UsuarioPermiso { Permiso = efPermiso1 }
        };
        var efUserGrupos = new List<EF.UsuarioGrupoPermisos>
        {
            new EF.UsuarioGrupoPermisos { GrupoPermiso = efGrupo1 }
        };

        var efUsuario = CrearUsuarioEF(10, "Juan", "Perez", efUserPermisos, efUserGrupos);

        Dom.Usuario domUsuario = DominioMapper.Map(efUsuario);

        Assert.Equal(10, domUsuario.IdUsuario);
        Assert.Equal("Juan", domUsuario.Nombre);
        Assert.Equal("ID10", domUsuario.Identificacion);

        Assert.NotEmpty(domUsuario.PermisosUsuario);
        Assert.Equal(1, domUsuario.PermisosUsuario.Count());
        Assert.Equal("EDITAR_PERFIL", domUsuario.PermisosUsuario.First().Nombre);

        Assert.NotEmpty(domUsuario.GrupoPermisos);
        Assert.Equal(1, domUsuario.GrupoPermisos.Count());
        Assert.Equal("Administradores", domUsuario.GrupoPermisos.First().Nombre);
    }


    // ===================================================================
    //       PRUEBA 2: MAPEO DE DOMINIO A DATOS (Escritura Segura)
    // ===================================================================

    [Fact]
    public void UsuarioMapper_DominioAData_IgnoraColeccionesMMyMapeaCore()
    {
        // ARRANGE
        // La colección PermisosUsuario NO se debe mapear de vuelta.
        var domUsuario = CrearUsuarioDom(20, "Laura", "Gomez",
            userPerms: new List<Dom.Permiso> { new Dom.Permiso { Nombre = "Borrar" } });

        // ACT
        EF.Usuario efUsuario = DominioMapper.Map(domUsuario);

        // ASSERT
        // 1. Verificación de campos simples
        Assert.Equal(20, efUsuario.IdUsuario);
        Assert.Equal("Laura", efUsuario.Nombre);
        Assert.Equal("dom_hash", efUsuario.Contrasenia); // Verifica que los datos core se transfieren.

        // 2. Verificación de Seguridad M:M (Las colecciones de unión deben ser ignoradas/vacías/no inicializadas)
        // Esto verifica que el [MapperIgnoreTarget] funcionó, y que no estamos sobrescribiendo la DB.
        Assert.Empty(efUsuario.UsuariosPermisos);
        Assert.Empty(efUsuario.UsuariosGruposPermisos);
    }
}