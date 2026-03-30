using MatriculasService.Application.Interfaces;

namespace MatriculasService.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly MatriculaDbContext _context;

    public UnitOfWork(MatriculaDbContext context)
    {
        _context = context;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(
            state: true,
            operation: async (_, _, ct) =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(ct);
                try
                {
                    await operation(ct);
                    await transaction.CommitAsync(ct);
                }
                catch
                {
                    await transaction.RollbackAsync(ct);
                    throw;
                }
                return true;
            },
            verifySucceeded: null,
            cancellationToken: cancellationToken);
    }
}
