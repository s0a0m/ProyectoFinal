using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System.Net.Http;
using System.Threading.Tasks;
using Dom = src.Models.Domain;
using src.Models.CodeFirst;

namespace Tests.Integration;

// La clase es PUBLIC para cumplir con el requisito de xUnit.
public class UsuariosControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private const string ApiUrl = "/api/Usuarios";

    // Constructor PUBLIC y ÚNICO, resolviendo el error de xUnit.
    public UsuariosControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            // Permitimos la redirección en caso de que su aplicación MVC lo haga, aunque en la API no debería.
            AllowAutoRedirect = false
        });
    }

    /// <summary>
    /// Prueba el ciclo de vida completo: Creación (201), Obtención (200), Eliminación (204) y Verificación (404).
    /// </summary>
    [Fact]
    public async Task CreateGetAndDeleteUser_ShouldWorkEndToEnd()
    {
        // ARRANGE: Preparar un usuario de prueba
        var newUser = GetTestUser();
        var jsonContent = CreateJsonContent(newUser);

        // 1. POST: Creación (Espera 201 Created)
        var postResponse = await _client.PostAsync(ApiUrl, jsonContent);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        // Obtener el ID creado (necesario para las siguientes llamadas)
        var createdUser = await DeserializeResponse<Dom.Usuario>(postResponse);
        Assert.Fail("JSON de Creación: " + JsonSerializer.Serialize(createdUser));
        Assert.NotNull(createdUser);
        var createdId = createdUser!.IdUsuario;
        Assert.True(createdId > 0);

        // 2. GET: Verificación de la Existencia (Espera 200 OK)
        var getResponse = await _client.GetAsync($"{ApiUrl}/{createdId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        // 3. DELETE: Eliminación (Espera 204 No Content)
        var deleteResponse = await _client.DeleteAsync($"{ApiUrl}/{createdId}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // 4. GET FINAL: Intentar obtener el usuario de nuevo (Espera 404 NotFound)
        var checkDeleteResponse = await _client.GetAsync($"{ApiUrl}/{createdId}");

        Assert.Equal(HttpStatusCode.NotFound, checkDeleteResponse.StatusCode);

        // Verificación en la BD en Memoria (Doble chequeo)
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userInDb = await context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == createdId);
            Assert.Null(userInDb);
        }
    }

    /// <summary>
    /// Prueba que un GET a un ID que no existe devuelve 404 NotFound.
    /// </summary>
    [Fact]
    public async Task GetById_ReturnsNotFound()
    {
        // ARRANGE: Usamos un ID que sabemos que no existirá en la BD limpia
        const int nonExistentId = 9999;

        // ACT
        var response = await _client.GetAsync($"{ApiUrl}/{nonExistentId}");

        // ASSERT
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    /// <summary>
    /// Prueba que el controlador rechaza un objeto Usuario con datos inválidos (p.ej., sin Correo).
    /// </summary>
    [Fact]
    public async Task Create_WithInvalidData_ReturnsBadRequest()
    {
        // ARRANGE: Crear un usuario inválido (sin un campo requerido, p.ej., Correo nulo)
        var invalidUser = GetTestUser();
        // Nota: Asumimos que Correo no puede ser nulo
        invalidUser.Correo = null!;

        var jsonContent = CreateJsonContent(invalidUser);

        // ACT
        var response = await _client.PostAsync(ApiUrl, jsonContent);

        // ASSERT
        // Espera un 400 Bad Request debido a fallas de validación del modelo
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }


    // --- MÉTODOS DE UTILIDAD ---

    private static StringContent CreateJsonContent(Dom.Usuario user)
    {
        return new StringContent(
            JsonSerializer.Serialize(user),
            Encoding.UTF8,
            "application/json"
        );
    }

    private static async Task<T?> DeserializeResponse<T>(HttpResponseMessage response) where T : class
    {
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(responseString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    private static Dom.Usuario GetTestUser()
    {
        // NOTA: ASEGÚRATE DE QUE ESTOS DATOS COINCIDAN CON LOS CAMPOS REQUERIDOS DE TU MODELO DOM.USUARIO
        return new Dom.Usuario
        {
            Correo = $"testuser_{Guid.NewGuid()}@prueba.com",
            Contrasenia = "Password123",
            Nombre = "Usuario",
            Apellido = "Prueba",
            Telefono = "123456789",
            Identificacion = Guid.NewGuid().ToString().Substring(0, 8),
            IdUsuario = 0, // El 0 indica que debe ser generado por la BD
        };
    }
}
