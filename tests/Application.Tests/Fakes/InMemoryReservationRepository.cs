// tests/Application.Tests/Fakes/InMemoryReservationRepository.cs
using Domain.Reservations;

namespace Application.Tests.Fakes;

public class InMemoryReservationRepository : IReservationRepository
{
    public List<Reservation> Reservations { get; } = [];

    public Task<IReadOnlyList<Reservation>> GetByCourtAsync(Guid courtId)
    {
        var result = Reservations.Where(r => r.CourtId == courtId).ToList();
        return Task.FromResult<IReadOnlyList<Reservation>>(result);
    }

    public Task AddAsync(Reservation reservation)
    {
        Reservations.Add(reservation);
        return Task.CompletedTask;
    }
}