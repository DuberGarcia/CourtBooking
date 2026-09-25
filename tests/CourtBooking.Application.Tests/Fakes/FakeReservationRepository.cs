using CourtBooking.Domain.Courts;
using CourtBooking.Domain.Reservations;

namespace CourtBooking.Application.Tests.Fakes;

public class FakeReservationRepository : IReservationRepository
{
    public List<Reservation> Saved { get; } = [];
    public Task AddAsync(Reservation reservation)
    {
        Saved.Add(reservation);
        return Task.CompletedTask;
    }

    public Task<List<Reservation>> GetByCourtAndDateAsync(Guid CourtId, DateOnly Start)
    {
        var filtradas = Saved.Where(r => r.CourtId == CourtId).ToList();
        return Task.FromResult(filtradas);
    }
}