using Microsoft.EntityFrameworkCore;
using src.Models.CodeFirst;
using src.Repositories.Interfaces;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;
using src.Models.Mappers;

namespace src.Repositories.Implementations;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public void LimpiarRastreador()
    {
        _context.ChangeTracker.Clear();
    }
    public void Dispose()
    {
        _context.Dispose();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
