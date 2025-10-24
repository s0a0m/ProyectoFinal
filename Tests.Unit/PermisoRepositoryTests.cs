using Microsoft.EntityFrameworkCore;
using System;
using EF = src.Models.CodeFirst;
using src.Repositories.Interfaces; // Para la interfaz IPermisoRepository
using RepoImpl = src.Repositories.Implementations; // ALIAS para la clase de implementación del Repositorio

// Usamos el namespace de tu proyecto de pruebas
namespace PermisoRepository.Tests;

public class PermisoRepositoryTests : IDisposable
{
    protected readonly EF.AppDbContext _context;

    // ERROR CORREGIDO: El tipo debe ser la interfaz IPermisoRepository, 
    // y el nombre del campo debe ser _repository (no PermisoRepositoryTests).
    protected readonly IPermisoRepository _repository;

    // Configuración de IDs de prueba
    protected const short TEST_USER_ID = 101;
    protected const int TEST_PERMISO_ID_READ = 201;
    protected const int TEST_PERMISO_ID_WRITE = 202;

    public PermisoRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<EF.AppDbContext>()
            // Usar GUID para asegurar una base de datos única por instancia de pruebas.
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new EF.AppDbContext(options);

        // ERROR CORREGIDO: Se usa el alias 'RepoImpl' para instanciar la clase PermisoRepository
        _repository = new RepoImpl.PermisoRepository(_context);

        // Cargar datos iniciales (Seed)
        SeedDatabase();
    }

    // Método para insertar datos de prueba necesarios
    private void SeedDatabase()
    {
        // Insertar un usuario
        _context.Usuarios.Add(new EF.Usuario { IdUsuario = TEST_USER_ID, Nombre = "Test", Apellido = "User", Correo = "test@user.com", Contrasenia = "pass", Identificacion = "id", FechaAlta = DateTime.Today, Telefono = "123" });

        // Insertar permisos
        _context.Permisos.Add(new EF.Permiso { IdPermiso = TEST_PERMISO_ID_READ, Nombre = "TEST_READ", Descripcion = "Permiso de lectura" });
        _context.Permisos.Add(new EF.Permiso { IdPermiso = TEST_PERMISO_ID_WRITE, Nombre = "TEST_WRITE", Descripcion = "Permiso de escritura" });

        // Insertar una relación existente (usuario 101 ya tiene permiso 201)
        _context.UsuariosPermisos.Add(new EF.UsuarioPermiso { IdUsuario = TEST_USER_ID, IdPermiso = TEST_PERMISO_ID_READ });

        _context.SaveChanges();
    }

    // Limpieza después de cada prueba
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}