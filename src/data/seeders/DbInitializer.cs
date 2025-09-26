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
        // var json = File.ReadAllText("data/seed/condiciones_pago.json");
        // var seedData = JsonSerializer.Deserialize<CondicionesPagoSeed>(json)!;

        // context.ChangeTracker.Clear();

        // // 1️⃣ Insertar solo la tabla base si no existen
        // foreach (var c in seedData.condiciones_pago)
        // {
        //     if (!context.Condicion_pagos.Any(cp => cp.id_condicion_pago == c.id_condicion_pago))
        //     {
        //         context.Condicion_pagos.Add(new condicion_pago
        //         {
        //             id_condicion_pago = c.id_condicion_pago,
        //             dias_pago = c.dias_pago
        //         });
        //     }
        // }
        // context.SaveChanges();

        // // 2️⃣ Insertar cuotas usando la fila base existente
        // foreach (var c in seedData.cuotas)
        // {
        //     if (!context.Cuota.Any(cu => cu.id_condicion_pago == c.id_condicion_pago))
        //     {
        //         // Traer la fila base existente
        //         var baseCp = context.Condicion_pagos.Find(c.id_condicion_pago);
        //         if (baseCp != null)
        //         {
        //             // Adjuntar la base para que EF no intente insertarla de nuevo
        //             context.Entry(baseCp).State = EntityState.Unchanged;

        //             context.Cuota.Add(new cuota
        //             {
        //                 id_condicion_pago = baseCp.id_condicion_pago,
        //                 dias_pago = c.dias_pago,
        //                 cuotas = c.cuotas,
        //                 interes_porcentual = c.interes_porcentual
        //             });
        //         }
        //     }
        // }
        // context.SaveChanges();

        // // 3️⃣ Insertar contados usando la fila base existente
        // foreach (var c in seedData.contados)
        // {
        //     if (!context.Contado.Any(co => co.id_condicion_pago == c.id_condicion_pago))
        //     {
        //         var baseCp = context.Condicion_pagos.Find(c.id_condicion_pago);
        //         if (baseCp != null)
        //         {
        //             context.Entry(baseCp).State = EntityState.Unchanged;

        //             context.Contado.Add(new contado
        //             {
        //                 id_condicion_pago = baseCp.id_condicion_pago,
        //                 dias_pago = c.dias_pago
        //             });
        //         }
        //     }
        // }
        // context.SaveChanges();
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
