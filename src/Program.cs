using Microsoft.EntityFrameworkCore;
using src.Core.Services.Implementations;
using src.Core.Services.Interfaces;
using src.External;
using src.Infrastructure.Repositories;
using src.Models.CodeFirst;
using src.Models.Mappers;
using src.Repositories.Implementations;
using src.Repositories.Interfaces;
using src.Repositories.Interfaces;
using src.Services.Implementations;

var builder = WebApplication.CreateBuilder(
    new WebApplicationOptions { Args = args, WebRootPath = "Presentation/wwwroot" }
);

var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");

// Add services to the container.
builder
    .Services.AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        // options.ViewLocationFormats.Clear(); // Opcional: limpiar las rutas por defecto si quieres control total
        options.ViewLocationFormats.Add("/Presentation/Views/{1}/{0}.cshtml");
        options.ViewLocationFormats.Add("/Presentation/Views/Shared/{0}.cshtml");
    });

// builder.Environment.WebRootPath = Path.Combine(builder.Environment.ContentRootPath, "Presentation", "wwwroot");
builder.Environment.WebRootPath = Path.Combine(
    Directory.GetCurrentDirectory(),
    "Presentation",
    "wwwroot"
);

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

// mappers
builder.Services.AddSingleton<ProvinciaMapper>();
builder.Services.AddSingleton<DomicilioMapper>();
builder.Services.AddSingleton<ProveedorMapper>();
builder.Services.AddSingleton<CodigoBarraMapper>();
builder.Services.AddSingleton<FamiliaMapper>();
builder.Services.AddSingleton<CategoriaMapper>();
builder.Services.AddSingleton<DepositoMapper>();
builder.Services.AddSingleton<EstanteMapper>();
builder.Services.AddSingleton<FilaMapper>();
builder.Services.AddSingleton<UbicacionProductoMapper>();
builder.Services.AddSingleton<ProductoMapper>();
builder.Services.AddSingleton<MovimientoStockMapper>();
builder.Services.AddSingleton<ComprobanteMapper>();

// repositorios
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
builder.Services.AddScoped<IProductoProveedorRepository, ProductoProveedorRepository>();
builder.Services.AddScoped<IExcelDataReader, ExcelDataAdapter>();
builder.Services.AddScoped<INovedadesRepository, NovedadesRepository>();
builder.Services.AddScoped<IFacturaRepository, FacturaRepository>();
builder.Services.AddScoped<INovedadesService, NovedadesService>();
builder.Services.AddScoped<IProductoProveedorService, ProductoProveedorService>();
builder.Services.AddScoped<ICompraRepository, CompraRepository>();
builder.Services.AddScoped<ICondicionPagoRepository, CondicionPagoRepository>();
builder.Services.AddScoped<IComprobanteRepository, ComprobanteRepository>();
builder.Services.AddScoped<IOrdenPagoRepository, OrdenPagoRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDepositoRepository,          DepositoRepository>();
builder.Services.AddScoped<IEstanteRepository,           EstanteRepository>();
builder.Services.AddScoped<IFilaRepository,              FilaRepository>();
builder.Services.AddScoped<IUbicacionProductoRepository, UbicacionProductoRepository>();




// builder.Services.AddScoped<ICompraRepository, CompraRepository>();
// Servicios
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrdenPagoService, OrdenPagoService>();
builder.Services.AddScoped<IComprobanteService, ComprobanteService>();
builder.Services.AddScoped<IGrupoPermisosService, GrupoPermisosService>();
builder.Services.AddHttpContextAccessor(); // Ya lo tenías
builder.Services.AddTransient<src.Presentation.Services.LayoutService>();
builder.Services.AddScoped<ICompraService, CompraService>();
builder.Services.AddScoped<IImportacionService, ImportacionService>();
builder.Services.AddScoped<IFacturaService, FacturaService>();
builder.Services.AddScoped<IOrdenPagoService, OrdenPagoService>();
builder.Services.AddScoped<INumeracionRepository, NumeracionRepository>();
builder.Services.AddScoped<INumeracionService, NumeracionService>();



builder.Services.AddScoped<IDepositoService, DepositoService>();
builder.Services.AddScoped<IEstanteService, EstanteService>();
builder.Services.AddScoped<IFilaService, FilaService>();
builder.Services.AddScoped<IStockService,    StockService>();

// Repositorios (agregar si aún no están registrados)




builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".MiSistema.Session";
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICartService, CartService>();

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
        System.Console.WriteLine(
            "Error al inicializar la base de datos con datos de prueba: " + ex.Message
        );
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Acceso}/{action=Login}/{id?}");

app.Run();

public partial class Program { }
