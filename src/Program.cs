using Microsoft.EntityFrameworkCore;
using src.Core.Services.Implementations;
using src.Core.Services.Interfaces;
using src.External;
using src.Models.CodeFirst;
using src.Repositories.Implementations;
using src.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "Presentation/wwwroot"
});


var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");
// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        // options.ViewLocationFormats.Clear(); // Opcional: limpiar las rutas por defecto si quieres control total
        options.ViewLocationFormats.Add("/Presentation/Views/{1}/{0}.cshtml");
        options.ViewLocationFormats.Add("/Presentation/Views/Shared/{0}.cshtml");
    });

// builder.Environment.WebRootPath = Path.Combine(builder.Environment.ContentRootPath, "Presentation", "wwwroot");
builder.Environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "Presentation", "wwwroot");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();
builder.Services.AddScoped<IProveedorService, ProveedorService>();
builder.Services.AddScoped<ICommonDataService, CommonDataService>();
builder.Services.AddScoped<IPermisoRepository, PermisoRepository>();
builder.Services.AddScoped<IProvinciaRepository, ProvinciaRepository>();
builder.Services.AddScoped<IGrupoPermisosRepository, GrupoPermisosRepository>();
builder.Services.AddScoped<IFamiliaRepository, FamiliaRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IFamiliaService, FamiliaService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IBarcodeAdapter, ZxIngBarcodeAdapter>();
builder.Services.AddScoped<IProductoCodigoExternoRepository, ProductoCodigoExternoRepository>();

// Servicios
builder.Services.AddScoped<IUserService, UserService>();
// builder.Services.AddScoped<IProveedorService, ProveedorService>();

// ¡AÑADIR ESTA LÍNEA PARA EL NUEVO SERVICIO!
builder.Services.AddScoped<IGrupoPermisosService, GrupoPermisosService>();
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