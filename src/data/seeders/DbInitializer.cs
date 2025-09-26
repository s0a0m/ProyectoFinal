using System.Text.Json;
using src.Models.CodeFirst;

public static class DbInitializer
{
    public static void Seed(AppDbContext context)
    {
        if (!context.Provincias.Any())
        {
            var json = File.ReadAllText("Data/Seed/provincias.json");
            var provincias = JsonSerializer.Deserialize<List<provincia>>(json)!;
            context.Provincias.AddRange(provincias);
        }

        context.SaveChanges();
    }
}
