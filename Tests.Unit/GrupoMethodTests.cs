using Xunit;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;

namespace GrupoPermisosRepository.Tests;

public class GrupoMethodTests : GrupoPermisosRepositoryTests
{
    // ----------------------------------------------------------------------
    // GetByIdAsync
    // ----------------------------------------------------------------------

    [Fact]
    public async Task GetByIdAsync_DebeRetornarGrupoConPermisosIncluidos()
    {
        // Act
        var result = await _repository.GetByIdAsync(TEST_GRUPO_ID_ADMIN);
        // Assert
        Assert.NotNull(result);

        // 1. Verificación de la clave primaria del Grupo (Usando IdGrupoPermiso)
        Assert.Equal(TEST_GRUPO_ID_ADMIN, result.IdGrupoPermiso);

        // 2. Verificación de la colección de Permisos
        // Debe haber un permiso (el 201 insertado en el seed)
        Assert.Single(result.Permisos);

        // 3. Verificación de la clave primaria del Permiso (Usando IdPermiso)
        Assert.Equal(TEST_PERMISO_ID_READ, result.Permisos.First().IdPermiso);
    }

    [Fact]
    public async Task GetByIdAsync_DebeRetornarNullSiGrupoNoExiste()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    // ----------------------------------------------------------------------
    // AddPermisoToGrupo
    // ----------------------------------------------------------------------

    [Fact]
    public async Task AddPermisoToGrupo_DebeAsignarNuevoPermiso()
    {
        // Arrange: El Grupo Readers (302) NO tiene el Permiso WRITE (202)

        // Act
        var result = await _repository.AddPermisoToGrupo(TEST_GRUPO_ID_READERS, TEST_PERMISO_ID_WRITE);

        // Assert
        Assert.True(result);
        // Verificar en la DB que la relación existe
        var nuevaRelacion = await _context.GruposPermisosPermisos
            .AnyAsync(gpp => gpp.IdGrupoPermiso == TEST_GRUPO_ID_READERS && gpp.IdPermiso == TEST_PERMISO_ID_WRITE);
        Assert.True(nuevaRelacion);
    }

    [Fact]
    public async Task AddPermisoToGrupo_DebeRetornarTrueSiRelacionYaExiste()
    {
        // Arrange: Grupo ADMIN (301) YA tiene Permiso READ (201) desde el seed

        // Act
        var result = await _repository.AddPermisoToGrupo(TEST_GRUPO_ID_ADMIN, TEST_PERMISO_ID_READ);

        // Assert
        Assert.True(result); // No es un error, simplemente ya estaba asignado
        // Verificar que solo sigue habiendo una relación 201
        var count = await _context.GruposPermisosPermisos
            .CountAsync(gpp => gpp.IdGrupoPermiso == TEST_GRUPO_ID_ADMIN && gpp.IdPermiso == TEST_PERMISO_ID_READ);
        Assert.Equal(1, count);
    }

    [Fact]
    // este test se concreta nada mas que no se produce la excepcion. Pero no se crea nada 
    public async Task AddPermisoToGrupo_DebeRetornarFalseEnErrorDeClaveForanea()
    {
        // Act
        // Asignar un permiso (202) a un grupo inexistente (9999).
        // NOTA: En EF Core In-Memory, las FK no fallan explícitamente como en una DB real.
        // Asumo que tu repositorio captura la excepción (si no hay validación previa) y retorna false.
        var result = await _repository.AddPermisoToGrupo(9999, TEST_PERMISO_ID_WRITE);
        // var a = await _repository.GetAllAsync();
        // Assert
        // Si el repositorio no valida la existencia, la DB fallaría. En el In-Memory, a menudo retorna false.
        Assert.False(result);
    }

    // ----------------------------------------------------------------------
    // RemovePermisoFromGrupo
    // ----------------------------------------------------------------------

    [Fact]
    public async Task RemovePermisoFromGrupo_DebeEliminarRelacionExistente()
    {
        // Arrange: Grupo ADMIN (301) tiene Permiso READ (201)

        // Act
        var result = await _repository.RemovePermisoFromGrupo(TEST_GRUPO_ID_ADMIN, TEST_PERMISO_ID_READ);

        // Assert
        Assert.True(result);
        // Verificar que la relación YA NO existe
        var relacionExiste = await _context.GruposPermisosPermisos
            .AnyAsync(gpp => gpp.IdGrupoPermiso == TEST_GRUPO_ID_ADMIN && gpp.IdPermiso == TEST_PERMISO_ID_READ);
        Assert.False(relacionExiste);
    }

    [Fact]
    public async Task RemovePermisoFromGrupo_DebeRetornarTrueSiRelacionNoExiste()
    {
        // Arrange: Grupo READERS (302) NO tiene Permiso READ (201)

        // Act
        var result = await _repository.RemovePermisoFromGrupo(TEST_GRUPO_ID_READERS, TEST_PERMISO_ID_READ);

        // Assert
        // El método debe ser idempotente: si no existe y no falla, retorna true.
        Assert.True(result);
    }

    // ----------------------------------------------------------------------
    // UpdateAsync
    // ----------------------------------------------------------------------

    [Fact]
    public async Task UpdateAsync_DebeActualizarNombreYDescripcion()
    {
        // Arrange
        const string NUEVO_NOMBRE = "ADMIN_NUEVO";
        var grupoDominio = MockDomain.CreateGrupo(TEST_GRUPO_ID_ADMIN, NUEVO_NOMBRE, "Nueva Descripción");

        // Act
        await _repository.UpdateAsync(grupoDominio);

        // Assert
        var efGrupo = await _context.GruposPermisos.FindAsync(TEST_GRUPO_ID_ADMIN);
        Assert.NotNull(efGrupo);
        Assert.Equal(NUEVO_NOMBRE, efGrupo.Nombre);
        Assert.Equal("Nueva Descripción", efGrupo.Descripcion);
    }

    [Fact]
    public async Task UpdateAsync_DebeLanzarExcepcionSiGrupoNoExiste()
    {
        // Arrange
        const short INEXISTENT_ID = 9999;
        var grupoDominio = MockDomain.CreateGrupo(INEXISTENT_ID, "Inexistente", "Descripción");

        // Act & Assert
        // El método UpdateAsync está configurado para lanzar KeyNotFoundException
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _repository.UpdateAsync(grupoDominio));
    }

    // ----------------------------------------------------------------------
    // DeleteAsync
    // ----------------------------------------------------------------------

    [Fact]
    public async Task DeleteAsync_DebeEliminarGrupoYRelacionesEnCascada()
    {
        // Arrange: Grupo ADMIN (301) existe y tiene 2 relaciones (GPP y UGP)

        // Act
        var result = await _repository.DeleteAsync(TEST_GRUPO_ID_ADMIN);

        // Assert
        Assert.True(result);

        // 1. Verificar que el grupo fue eliminado
        var grupoExiste = await _context.GruposPermisos.AnyAsync(g => g.IdGrupoPermiso == TEST_GRUPO_ID_ADMIN);
        Assert.False(grupoExiste);

        // 2. Verificar que la relación GPP fue eliminada (Cascade Check)
        var gppExiste = await _context.GruposPermisosPermisos.AnyAsync(gpp => gpp.IdGrupoPermiso == TEST_GRUPO_ID_ADMIN);
        Assert.False(gppExiste);

        // 3. Verificar que la relación UGP fue eliminada (Cascade Check)
        var ugpExiste = await _context.UsuariosGruposPermisos.AnyAsync(ugp => ugp.IdGrupoPermiso == TEST_GRUPO_ID_ADMIN);
        Assert.False(ugpExiste);
    }

    [Fact]
    public async Task DeleteAsync_DebeRetornarFalseSiGrupoNoExiste()
    {
        // Act
        var result = await _repository.DeleteAsync(9999);

        // Assert
        // El método DeleteAsync está configurado para retornar false si no encuentra el grupo.
        Assert.False(result);
    }
}