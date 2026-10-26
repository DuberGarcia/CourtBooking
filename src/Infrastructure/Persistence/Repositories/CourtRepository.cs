using Domain.Courts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class CourtRepository : ICourtRepository
{
    private readonly CourtBookingDbContext _context;
    public CourtRepository(CourtBookingDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Court court)
    {
        _context.Courts.Add(court);
        await _context.SaveChangesAsync();
    }

    public async Task<Court?> GetByIdAsync(Guid id)
    {
        return await _context.Courts.FirstOrDefaultAsync(c => c.Id==id);
    }
}