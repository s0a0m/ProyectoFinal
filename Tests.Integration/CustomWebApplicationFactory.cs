using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using src.Models.CodeFirst;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using src;

namespace Tests.Integration;

// CLASE PUBLICA: Solución definitiva a los errores CS0051 y CS0060.
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // 1. ELIMINAR la configuración de la BD persistente (PostgreSQL)
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            // 2. AÑADIR la base de datos en memoria (Aislamiento de prueba)
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            // 3. Inicializar el esquema de la BD en memoria para la prueba
            var serviceProvider = services.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var context = scopedServices.GetRequiredService<AppDbContext>();

                // Borra y crea el esquema en memoria en cada inicio de la fábrica
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        });
    }
}