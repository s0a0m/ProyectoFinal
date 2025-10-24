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
            SeedUsuarios(context);
            context.SaveChanges();
            SeedDomicilios(context);
            context.SaveChanges();
            SeedProveedores(context);
            context.SaveChanges();

            ResetSequence(context, "provincia", "id_provincia");
            ResetSequence(context, "condicion_pago", "id_condicion_pago");
            ResetSequence(context, "domicilio", "id_domicilio");
            ResetSequence(context, "proveedor", "id_proveedor");
            ResetSequence(context, "usuario", "id_usuario");

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

    private static void SeedProvincias(AppDbContext context)
    {
        if (context.Provincias.Any()) return;

        var json = File.ReadAllText("data/seed/provincias.json");
        var provincias = JsonSerializer.Deserialize<List<Provincia>>(json, _jsonOptions)!;
        context.Provincias.AddRange(provincias);
        Console.WriteLine($"- Seeding {provincias.Count} provincias...");
    }

    private static void SeedCondicionesPago(AppDbContext context)
    {
        if (context.Condicion_pagos.Any()) return;

        var json = File.ReadAllText("data/seed/condiciones_pago.json");
        var data = JsonSerializer.Deserialize<CondicionesPagoFile>(json, _jsonOptions)!;

        context.Condicion_pagos.AddRange(data.Cuotas);
        context.Condicion_pagos.AddRange(data.Contados);
        Console.WriteLine($"- Seeding {data.Cuotas.Count} Cuotas y {data.Contados.Count} Contados...");
    }

    private static void SeedDomicilios(AppDbContext context)
    {
        if (context.Domicilios.Any()) return;

        var json = File.ReadAllText("data/seed/domicilios.json");
        var domicilios = JsonSerializer.Deserialize<List<Domicilio>>(json, _jsonOptions)!;
        context.Domicilios.AddRange(domicilios);
        Console.WriteLine($"- Seeding {domicilios.Count} domicilios...");
    }

    private static void SeedProveedores(AppDbContext context)
    {
        if (context.Proveedores.Any()) return;

        var json = File.ReadAllText("data/seed/proveedores.json");
        var proveedores = JsonSerializer.Deserialize<List<Proveedor>>(json, _jsonOptions)!;
        context.Proveedores.AddRange(proveedores);
        Console.WriteLine($"- Seeding {proveedores.Count} proveedores...");
    }

    private static void SeedUsuarios(AppDbContext context)
    {
        if (context.Usuarios.Any()) return;

        var json = File.ReadAllText("data/seed/usuarios.json");
        var usuarios = JsonSerializer.Deserialize<List<Usuario>>(json, _jsonOptions)!;
        context.Usuarios.AddRange(usuarios);
        Console.WriteLine($"- Seeding {usuarios.Count} usuarios...");
    }
}
