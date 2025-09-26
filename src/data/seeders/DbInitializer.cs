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
            var provincias = JsonSerializer.Deserialize<List<provincia>>(json)!;
            context.Provincias.AddRange(provincias);
        }

        context.SaveChanges();
    }
    public static void SeedCondicionesPago(AppDbContext context)
    {
        var json = File.ReadAllText("data/seed/condiciones_pago.json");
        var seedData = JsonSerializer.Deserialize<CondicionesPagoSeed>(json)!;

        context.ChangeTracker.Clear();

        // Insertar solo si no existen
        if (!context.Condicion_pagos.Any())
            context.Condicion_pagos.AddRange(seedData.condiciones_pago.Select(c => new condicion_pago
            {
                id_condicion_pago = c.id_condicion_pago,
                dias_pago = c.dias_pago
            }));

        if (!context.Cuota.Any())
            context.Cuota.AddRange(seedData.cuotas.Select(c => new cuota
            {
                id_condicion_pago = c.id_condicion_pago,
                dias_pago = c.dias_pago,
                cuotas = c.cuotas,
                interes_porcentual = c.interes_porcentual
            }));

        if (!context.Contado.Any())
            context.Contado.AddRange(seedData.contados.Select(c => new contado
            {
                id_condicion_pago = c.id_condicion_pago,
                dias_pago = c.dias_pago
            }));

        context.SaveChanges();
    }
    public static void SeedDomicilios(AppDbContext context)
    {
        if (!context.Domicilios.Any())
        {
            context.ChangeTracker.Clear(); // limpia el tracker
            var json = File.ReadAllText("data/seed/domicilios.json");
            var domicilios = JsonSerializer.Deserialize<List<domicilio>>(json)!;
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
            var proveedores = JsonSerializer.Deserialize<List<proveedor>>(json)!;
            context.Proveedores.AddRange(proveedores);
            context.SaveChanges();
        }
    }
    private class CondicionesPagoSeed
    {
        public List<condicion_pago> condiciones_pago { get; set; } = new();
        public List<cuota> cuotas { get; set; } = new();
        public List<contado> contados { get; set; } = new();
    }
}
