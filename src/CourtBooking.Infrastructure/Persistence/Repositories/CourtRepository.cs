using CourtBooking.Domain.Courts;
using Microsoft.EntityFrameworkCore;

namespace CourtBooking.Infrastructure.Persistence.Repositories;

public class CourtRepository : ICourtRepository
{
    private readonly AppDbContext _context;

    public CourtRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Court?> GetByIdAsync(Guid id)
    {
        return await _context.Courts.FirstOrDefaultAsync(c => c.Id == id);
    }
}