using Application.Common.Interfaces;

namespace Infrastructure.Persistence;

public class Transaction : ITransaction
{
    private readonly CourtBookingDbContext _context;
    public Transaction(CourtBookingDbContext context)
    {
        _context = context;
    }
    public async Task<T> RunAsync<T>(Func<Task<T>> work)
    {
        await using var tx = await _context.Database.BeginTransactionAsync();
        var result = await work();
        await tx.CommitAsync();
        return result;
    }
}