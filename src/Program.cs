using Microsoft.EntityFrameworkCore;
using src.Core.Services.Interfaces;
using src.Models.CodeFirst;
using src.Repositories.Implementations;
using src.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

// builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// swagger

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // swagger
    app.UseSwagger();
    app.UseSwaggerUI();
    // seed prueba para proveedores
    using var scope = app.Services.CreateScope();
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        // context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        DbInitializer.SeedAll(context);
    }
    catch (Exception ex)
    {
        // var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        // logger.LogError(ex, "Error al inicializar la base de datos con datos de prueba.");
        System.Console.WriteLine("Error al inicializar la base de datos con datos de prueba: " + ex.Message);
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
public partial class Program { }