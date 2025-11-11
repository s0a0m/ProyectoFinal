using Microsoft.EntityFrameworkCore;
using System;
using EF = src.Models.CodeFirst;
using Dom = src.Models.Domain;
using src.Repositories.Interfaces;
using RepoImpl = src.Repositories.Implementations;

// Usamos el namespace de tu proyecto de pruebas
namespace GrupoPermisosRepository.Tests;

// Usaremos un alias para simular el mapeo del Dominio dentro de los tests
public class MockDomain
{
    // Función de ayuda para crear un modelo de Dominio simple (simulando el mapeo del Repositorio)
    public static Dom.GrupoPermisos CreateGrupo(short id, string nombre, string descripcion, IEnumerable<Dom.Permiso> permisos = null)
    {
        // NOTA: ASUME que Dom.GrupoPermisos tiene este constructor.
        return new Dom.GrupoPermisos(id, nombre, descripcion, permisos ?? new List<Dom.Permiso>());
    }
}


public class GrupoPermisosRepositoryTests : IDisposable
{
    protected readonly EF.AppDbContext _context;
    protected readonly IGrupoPermisosRepository _repository;
    // Constantes de prueba
    protected const short TEST_USER_ID = 101;
    protected const int TEST_PERMISO_ID_READ = 201;
    protected const int TEST_PERMISO_ID_WRITE = 202;
    protected const short TEST_GRUPO_ID_ADMIN = 301;
    protected const short TEST_GRUPO_ID_READERS = 302;


    public GrupoPermisosRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<EF.AppDbContext>()
            // Usar un GUID para asegurar una base de datos única por instancia de pruebas.
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new EF.AppDbContext(options);

        // Instancia la implementación real del repositorio
        _repository = new RepoImpl.GrupoPermisosRepository(_context);

        // Cargar datos iniciales (Seed)
        SeedDatabase();
    }

    // Método para insertar datos de prueba necesarios
    private void SeedDatabase()
    {
        // 1. Insertar usuario de prueba
        _context.Usuarios.Add(new EF.Usuario { IdUsuario = TEST_USER_ID, Nombre = "Test", Apellido = "User", Correo = "test@user.com", Contrasenia = "pass", Identificacion = "id", FechaAlta = DateTime.Today, Telefono = "123" });

        // 2. Insertar permisos
        _context.Permisos.Add(new EF.Permiso { IdPermiso = TEST_PERMISO_ID_READ, Nombre = "TEST_READ", Descripcion = "Permiso de lectura" });
        _context.Permisos.Add(new EF.Permiso { IdPermiso = TEST_PERMISO_ID_WRITE, Nombre = "TEST_WRITE", Descripcion = "Permiso de escritura" });

        // 3. Insertar grupos
        _context.GruposPermisos.Add(new EF.GrupoPermisos { IdGrupoPermiso = TEST_GRUPO_ID_ADMIN, Nombre = "ADMIN", Descripcion = "Grupo administrador" });
        _context.GruposPermisos.Add(new EF.GrupoPermisos { IdGrupoPermiso = TEST_GRUPO_ID_READERS, Nombre = "READERS", Descripcion = "Grupo lectores" });

        // 4. Insertar relaciones iniciales
        // Relación: Permiso READ (201) asignado a Grupo ADMIN (301)
        _context.GruposPermisosPermisos.Add(new EF.GrupoPermisoPermiso { IdGrupoPermiso = TEST_GRUPO_ID_ADMIN, IdPermiso = TEST_PERMISO_ID_READ });

        // Relación: Grupo ADMIN (301) asignado a Usuario (101)
        _context.UsuariosGruposPermisos.Add(new EF.UsuarioGrupoPermisos { IdUsuario = TEST_USER_ID, IdGrupoPermiso = TEST_GRUPO_ID_ADMIN });

        _context.SaveChanges();
    }

    // Limpieza después de cada prueba
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}