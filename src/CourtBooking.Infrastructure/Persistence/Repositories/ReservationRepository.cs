using CourtBooking.Domain.Reservations;
using Microsoft.EntityFrameworkCore;

namespace CourtBooking.Infrastructure.Persistence.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly AppDbContext _context;

    public ReservationRepository(AppDbContext context)
    {
        _context=context;
    }
    public async Task AddAsync(Reservation reservation)
    {
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Reservation>> GetByCourtAndDateAsync(Guid CourtId, DateOnly Start)
    {
        var inicioDelDia = Start.ToDateTime(TimeOnly.MinValue);   // 2026-10-01 00:00
        var inicioDelSiguiente = inicioDelDia.AddDays(1);  
        return await _context.Reservations
            .Where(r => 
                r.Period.Start >= inicioDelDia && 
                r.Period.Start < inicioDelSiguiente &&
                r.CourtId == CourtId)
            .ToListAsync();
    }
}