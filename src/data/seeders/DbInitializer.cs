using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using src.Models.CodeFirst;

public static class DbInitializer
{
    public static void SeedProvincias(AppDbContext context)
    {
        if (!context.Provincias.Any())
        {
            context.ChangeTracker.Clear();
            var json = File.ReadAllText("data/seed/provincias.json");
            var provincias = JsonSerializer.Deserialize<List<Provincia>>(json)!;
            context.Provincias.AddRange(provincias);
        }

        context.SaveChanges();
    }
    public static void SeedCondicionesPago(AppDbContext context)
    {
        if (context.Condicion_pagos.Any())
            return; // Ya hay datos, no vuelvas a cargarlos

        string json = File.ReadAllText("data/seed/condiciones_pago.json");

        using var document = JsonDocument.Parse(json);

        var root = document.RootElement;

        // Leer las secciones del JSON
        var cuotasArray = root.GetProperty("cuotas").EnumerateArray();
        var contadosArray = root.GetProperty("contados").EnumerateArray();

        var condiciones = new List<CondicionDePago>();

        // Procesar cuotas
        foreach (var element in cuotasArray)
        {
            var c = new Cuota
            {
                IdCondicionPago = element.GetProperty("id_condicion_pago").GetInt16(),
                DiasPago = element.GetProperty("dias_pago").GetInt16(),
                Cuotas = element.GetProperty("cuotas").GetInt16(),
                InteresPorcentual = element.GetProperty("interes_porcentual").GetDecimal()
            };
            condiciones.Add(c);
        }

        // Procesar contados
        foreach (var element in contadosArray)
        {
            var c = new Contado
            {
                IdCondicionPago = element.GetProperty("id_condicion_pago").GetInt16(),
                DiasPago = element.GetProperty("dias_pago").GetInt16()
            };
            condiciones.Add(c);
        }

        context.Condicion_pagos.AddRange(condiciones);
        context.SaveChanges();

    }



    public static void SeedDomicilios(AppDbContext context)
    {
        if (!context.Domicilios.Any())
        {
            context.ChangeTracker.Clear(); // limpia el tracker
            var json = File.ReadAllText("data/seed/domicilios.json");
            var domicilios = JsonSerializer.Deserialize<List<Domicilio>>(json)!;
            context.Domicilios.AddRange(domicilios);
            context.SaveChanges();
        }
    }

    public static void SeedProveedores(AppDbContext context)
    {
        if (!context.Proveedores.Any())
        {
            context.ChangeTracker.Clear(); // limpia el tracker
            var json = File.ReadAllText("data/seed/proveedores.json");
            var proveedores = JsonSerializer.Deserialize<List<Proveedor>>(json)!;
            context.Proveedores.AddRange(proveedores);
            context.SaveChanges();
        }
    }
    public static void SeedUsuarios(AppDbContext context)
    {
        if (!context.Usuarios.Any())
        {
            context.ChangeTracker.Clear();
            var json = File.ReadAllText("data/seed/usuarios.json");
            var usuarios = JsonSerializer.Deserialize<List<Usuario>>(json)!;
            context.Usuarios.AddRange(usuarios);
            context.SaveChanges();
        }
    }
    private class CondicionesPagoSeed
    {
        public List<CondicionDePago> condiciones_pago { get; set; } = new();
        public List<Cuota> cuotas { get; set; } = new();
        public List<Contado> contados { get; set; } = new();
    }
}
