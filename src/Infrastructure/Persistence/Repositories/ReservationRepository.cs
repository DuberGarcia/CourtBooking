using Domain.Reservations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly CourtBookingDbContext _context;
    public ReservationRepository(CourtBookingDbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(Reservation reservation)
    {
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Reservation>> GetByCourtAsync(Guid courtId)
    {
        return await _context.Reservations
            .AsNoTracking()
            .Where(r => r.CourtId == courtId)
            .ToListAsync();
    }

    public async Task<Reservation?> GetByIdAsync(Guid id)
    {
        return await _context.Reservations
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task UpdateAsync(Reservation reservation)
    {
        _context.Reservations.Update(reservation);
        await _context.SaveChangesAsync();
    }
}