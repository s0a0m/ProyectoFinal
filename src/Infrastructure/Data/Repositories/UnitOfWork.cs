using Microsoft.EntityFrameworkCore.Storage;
using src.Models.CodeFirst;
using src.Repositories.Interfaces;

namespace src.Repositories.Implementations;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    private IDbContextTransaction? _currentTransaction; // El ? es por si es null

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public void LimpiarRastreador()
    {
        _context.ChangeTracker.Clear();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync()
    {
        _currentTransaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        try
        {
            await _context.SaveChangesAsync();

            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync();
            }
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();

                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.RollbackAsync();

            await _currentTransaction.DisposeAsync();

            _currentTransaction = null;
        }
    }
}
