using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using src.Models.CodeFirst;

public static class DbInitializer
{
    private class CondicionesPagoFile
    {
        public List<Cuota> Cuotas { get; set; } = new();
        public List<Contado> Contados { get; set; } = new();
    }

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        // PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
    };

    public static void SeedAll(AppDbContext context)
    {
        using var transaction = context.Database.BeginTransaction();

        try
        {
            SeedProvincias(context);
            SeedCondicionesPago(context);
            SeedPermisos(context);
            SeedUsuarios(context);
            SeedFamilia(context);
            SeedCodigoBarra(context);
            context.SaveChanges();
            SeedUsuarioPermisos(context);
            SeedDomicilios(context);
            SeedProductos(context);
            context.SaveChanges();
            SeedProveedores(context);
            SeedGrupoPermiso(context);
            context.SaveChanges();
            SeedCompras(context);
            SeedProductoProveedores(context);
            context.SaveChanges();
            SeedFacturas(context);
            SeedProductoCodigosExternos(context);
            context.SaveChanges();

            ResetSequence(context, "provincia", "id_provincia");
            ResetSequence(context, "condicion_pago", "id_condicion_pago");
            ResetSequence(context, "domicilio", "id_domicilio");
            ResetSequence(context, "proveedor", "id_proveedor");
            ResetSequence(context, "usuario", "id_usuario");
            ResetSequence(context, "familia", "id_familia");
            ResetSequence(context, "codigo_barra", "id_codigo_barra");
            ResetSequence(context, "producto", "id_producto");
            ResetSequence(context, "grupo_permisos", "id_grupo_permiso");
            ResetSequence(context, "compra", "id_compra");
            ResetSequence(context, "factura", "id_factura");
            ResetSequence(context, "detalle_factura", "id_detalle_factura");
            ResetSequence(context, "novedades_proveedor", "id_novedad");

            transaction.Commit();
            Console.WriteLine(">>> Seeding de base de datos completado exitosamente.");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Console.WriteLine($">>> ERROR en el seeding: {ex.Message}");
        }
    }

    private static void ResetSequence(AppDbContext context, string tableName, string idColumn)
    {
        if (tableName.Contains(';') || idColumn.Contains(';'))
            throw new ArgumentException("Nombre de tabla o columna inválido.");

        string sql = $"SELECT setval(pg_get_serial_sequence('{tableName}', '{idColumn}'), (SELECT MAX({idColumn}) FROM {tableName}) + 1);";
        context.Database.ExecuteSqlRaw(sql);
    }

    private static void SeedPermisos(AppDbContext context)
    {
        if (context.Permisos.Any()) return;
        var json = File.ReadAllText("Infrastructure/Data/Seeders/seed/permisos.json");
        var permisos = JsonSerializer.Deserialize<List<Permiso>>(json, _jsonOptions)!;
        context.Permisos.AddRange(permisos);
        Console.WriteLine($"- Seeding {permisos.Count} permisos...");
    }
    private static void SeedUsuarioPermisos(AppDbContext context)
    {
        if (context.UsuariosPermisos.Any()) return;
        var json = File.ReadAllText("Infrastructure/Data/Seeders/seed/usuario_permiso.json");
        var usuarioPermisos = JsonSerializer.Deserialize<List<UsuarioPermiso>>(json, _jsonOptions)!;
        context.ChangeTracker.AutoDetectChangesEnabled = false;
        context.UsuariosPermisos.AddRange(usuarioPermisos);
        context.ChangeTracker.AutoDetectChangesEnabled = true;
        Console.WriteLine($"- Seeding {usuarioPermisos.Count} relaciones UsuarioPermiso...");
    }
    private static void SeedProvincias(AppDbContext context)
    {
        if (context.Provincias.Any()) return;

        var json = File.ReadAllText("Infrastructure/Data/Seeders/seed/provincias.json");
        var provincias = JsonSerializer.Deserialize<List<Provincia>>(json, _jsonOptions)!;
        context.Provincias.AddRange(provincias);
        Console.WriteLine($"- Seeding {provincias.Count} provincias...");
    }

    private static void SeedCondicionesPago(AppDbContext context)
    {
        if (context.Condicion_pagos.Any()) return;

        var json = File.ReadAllText("Infrastructure/Data/Seeders/seed/condiciones_pago.json");
        var data = JsonSerializer.Deserialize<CondicionesPagoFile>(json, _jsonOptions)!;

        context.Condicion_pagos.AddRange(data.Cuotas);
        context.Condicion_pagos.AddRange(data.Contados);
        Console.WriteLine($"- Seeding {data.Cuotas.Count} Cuotas y {data.Contados.Count} Contados...");
    }

    private static void SeedDomicilios(AppDbContext context)
    {
        if (context.Domicilios.Any()) return;

        var json = File.ReadAllText("Infrastructure/Data/Seeders/seed/domicilios.json");
        var domicilios = JsonSerializer.Deserialize<List<Domicilio>>(json, _jsonOptions)!;
        context.Domicilios.AddRange(domicilios);
        Console.WriteLine($"- Seeding {domicilios.Count} domicilios...");
    }

    private static void SeedProveedores(AppDbContext context)
    {
        if (context.Proveedores.Any()) return;

        var json = File.ReadAllText("Infrastructure/Data/Seeders/seed/proveedores.json");
        var proveedores = JsonSerializer.Deserialize<List<Proveedor>>(json, _jsonOptions)!;
        context.Proveedores.AddRange(proveedores);
        Console.WriteLine($"- Seeding {proveedores.Count} proveedores...");
    }

    private static void SeedUsuarios(AppDbContext context)
    {
        if (context.Usuarios.Any()) return;

        var json = File.ReadAllText("Infrastructure/Data/Seeders/seed/usuarios.json");
        var usuarios = JsonSerializer.Deserialize<List<Usuario>>(json, _jsonOptions)!;
        context.Usuarios.AddRange(usuarios);
        Console.WriteLine($"- Seeding {usuarios.Count} usuarios...");
    }
    private static void SeedGrupoPermiso(AppDbContext context)
    {
        if (context.GruposPermisos.Any()) return;

        var json = File.ReadAllText("Infrastructure/Data/Seeders/seed/grupo_permiso.json");
        var grupoPermiso = JsonSerializer.Deserialize<List<GrupoPermisos>>(json, _jsonOptions)!;
        context.GruposPermisos.AddRange(grupoPermiso);
        Console.WriteLine($"- Seeding {grupoPermiso.Count} grupoPermiso...");
    }
    private static void SeedFamilia(AppDbContext context)
    {
        if (context.Familias.Any()) return;

        var json = File.ReadAllText("Infrastructure/Data/Seeders/seed/familias.json");
        var familias = JsonSerializer.Deserialize<List<Familia>>(json, _jsonOptions)!;
        context.Familias.AddRange(familias);
        Console.WriteLine($"- Seeding {familias.Count} familias...");
    }
    private static void SeedCodigoBarra(AppDbContext context)
    {
        if (context.CodigoBarras.Any()) return;

        var json = File.ReadAllText("Infrastructure/Data/Seeders/seed/codigos_de_barra.json");
        var codigoBarras = JsonSerializer.Deserialize<List<CodigoBarra>>(json, _jsonOptions)!;
        context.CodigoBarras.AddRange(codigoBarras);
        Console.WriteLine($"- Seeding {codigoBarras.Count} codigoBarras...");
    }
    private static void SeedProductos(AppDbContext context)
    {
        if (context.Productos.Any()) return;

        var json = File.ReadAllText("Infrastructure/Data/Seeders/seed/productos.json");
        var productos = JsonSerializer.Deserialize<List<Producto>>(json, _jsonOptions)!;
        context.Productos.AddRange(productos);
        Console.WriteLine($"- Seeding {productos.Count} productos...");
    }
    private static void SeedCompras(AppDbContext context)
    {
        if (context.Compras.Any()) return;
        var json = File.ReadAllText("Infrastructure/Data/Seeders/seed/compra_con_detalle.json");
        var compras = JsonSerializer.Deserialize<List<Compra>>(json, _jsonOptions)!;
        foreach (var compra in compras)
        {
            compra.FechaCompra = DateTime.SpecifyKind(compra.FechaCompra, DateTimeKind.Utc);
        }
        context.Compras.AddRange(compras);
        context.SaveChanges();

        Console.WriteLine($"- Seeding {compras.Count} compras con sus detalles...");
    }
    private static void SeedFacturas(AppDbContext context)
    {
        if (context.Facturas.Any()) return;
        var json = File.ReadAllText("Infrastructure/Data/Seeders/seed/factura_con_detalle.json");
        var facturas = JsonSerializer.Deserialize<List<Factura>>(json, _jsonOptions)!;
        foreach (var factura in facturas)
        {
            factura.FechaEmision = DateTime.SpecifyKind(factura.FechaEmision, DateTimeKind.Utc);
        }

        context.Facturas.AddRange(facturas);
        context.SaveChanges();

        Console.WriteLine($"- Seeding {facturas.Count} facturas con éxito.");
    }
    private static void SeedProductoProveedores(AppDbContext context)
    {
        if (context.ProductosProveedores.Any()) return;
        var json = File.ReadAllText("Infrastructure/Data/Seeders/seed/producto_proveedor.json");
        var productoProveedores = JsonSerializer.Deserialize<List<ProductoProveedor>>(json, _jsonOptions)!;
        context.ProductosProveedores.AddRange(productoProveedores);
        context.SaveChanges();

        Console.WriteLine($"- Seeding {productoProveedores.Count} producto proveedores con éxito.");
    }
    private static void SeedProductoCodigosExternos(AppDbContext context)
    {
        if (context.ProductoCodigosExternos.Any()) return;
        var json = File.ReadAllText("Infrastructure/Data/Seeders/seed/producto_codigo_externo.json");
        var items = JsonSerializer.Deserialize<List<ProductoCodigoExterno>>(json, _jsonOptions)!;

        context.ProductoCodigosExternos.AddRange(items);
        context.SaveChanges();

        Console.WriteLine($"- Seeding {items.Count} códigos externos de proveedores...");
    }
}

