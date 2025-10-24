using Xunit; // O NUnit/MSTest
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using EF = src.Models.CodeFirst;

namespace PermisoRepository.Tests;

public class PermisoMethodTests : PermisoRepositoryTests
{
    // ----------------------------------------------------------------------
    // GetPermisosByUsuarioIdAsync
    // ----------------------------------------------------------------------

    [Fact]
    public async Task GetPermisosByUsuarioIdAsync_DebeRetornarPermisosAsignados()
    {
        // Act
        var result = await _repository.GetPermisosByUsuarioIdAsync(TEST_USER_ID);

        // Assert
        Assert.NotNull(result);
        var permisos = result.ToList();

        // Debe haber un permiso (el 201 insertado en el seed)
        Assert.Single(permisos);
        Assert.Equal(TEST_PERMISO_ID_READ, permisos.First().IdPermiso);
    }

    [Fact]
    public async Task GetPermisosByUsuarioIdAsync_DebeRetornarListaVaciaSiUsuarioNoTienePermisos()
    {
        // Arrange: Creamos un usuario sin permisos
        const short NO_PERMISSION_USER_ID = 999;
        _context.Usuarios.Add(new EF.Usuario { IdUsuario = NO_PERMISSION_USER_ID, Nombre = "No", Apellido = "Perm", Correo = "no@perm.com", Contrasenia = "p", Identificacion = "999", FechaAlta = DateTime.Today, Telefono = "1" });
        _context.SaveChanges();

        // Act
        var result = await _repository.GetPermisosByUsuarioIdAsync(NO_PERMISSION_USER_ID);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    // ----------------------------------------------------------------------
    // AsignarPermisoAUsuario
    // ----------------------------------------------------------------------

    [Fact]
    public async Task AsignarPermisoAUsuario_DebeAsignarNuevoPermiso()
    {
        // Arrange: El usuario 101 NO tiene el permiso 202

        // Act
        var result = await _repository.AsignarPermisoAUsuario(TEST_PERMISO_ID_WRITE, TEST_USER_ID);

        // Assert
        Assert.True(result);
        // Verificar en la DB que la relación existe
        var nuevaRelacion = await _context.UsuariosPermisos
            .AnyAsync(up => up.IdUsuario == TEST_USER_ID && up.IdPermiso == TEST_PERMISO_ID_WRITE);
        Assert.True(nuevaRelacion);
    }

    [Fact]
    public async Task AsignarPermisoAUsuario_DebeRetornarTrueSiPermisoYaExiste()
    {
        // Arrange: El usuario 101 YA tiene el permiso 201 (desde el seed)

        // Act
        var result = await _repository.AsignarPermisoAUsuario(TEST_PERMISO_ID_READ, TEST_USER_ID);

        // Assert
        Assert.True(result); // No es un error, simplemente ya estaba asignado
        // Verificar que solo sigue habiendo una relación 201
        var count = await _context.UsuariosPermisos
            .CountAsync(up => up.IdUsuario == TEST_USER_ID && up.IdPermiso == TEST_PERMISO_ID_READ);
        Assert.Equal(1, count);
    }

    // [Prueba Negativa]
    [Fact]
    public async Task AsignarPermisoAUsuario_DebeRetornarFalseSiUsuarioNoExiste()
    {
        // Act
        var result = await _repository.AsignarPermisoAUsuario(TEST_PERMISO_ID_WRITE, 9999); // Usuario ID inexistente

        // Assert
        // Si el DbContext está configurado correctamente, esto generará DbUpdateException y retornará false
        Assert.False(result, result.ToString());
    }

    // ----------------------------------------------------------------------
    // AsignarPermisosAUsuario
    // ----------------------------------------------------------------------

    [Fact]
    public async Task AsignarPermisosAUsuario_DebeAgregarSoloPermisosFaltantes()
    {
        // Arrange: Asignaremos {201 (EXISTE), 202 (FALTA), 999 (INEXISTENTE)}
        var idsParaAsignar = new List<int> { TEST_PERMISO_ID_READ, TEST_PERMISO_ID_WRITE, 999 };

        // Act
        var result = await _repository.AsignarPermisosAUsuario(idsParaAsignar, TEST_USER_ID);

        // Assert
        Assert.True(result);

        // Verificar que solo se agregaron las relaciones válidas (202)
        var totalRelaciones = await _context.UsuariosPermisos.CountAsync(up => up.IdUsuario == TEST_USER_ID);
        // Ya tenía 201, se agregó 202 -> Total 2
        Assert.Equal(2, totalRelaciones);

        // Verificar que 202 fue agregado
        var isWriteAssigned = await _context.UsuariosPermisos
            .AnyAsync(up => up.IdUsuario == TEST_USER_ID && up.IdPermiso == TEST_PERMISO_ID_WRITE);
        Assert.True(isWriteAssigned);
    }
}