// tests/Application.Tests/Fakes/InMemoryReservationRepository.cs
using Domain.Reservations;

namespace Application.Tests.Fakes;

public class InMemoryReservationRepository : IReservationRepository
{
    public List<Reservation> Reservations { get; } = [];

    // Reservas que el caso de uso mandó a guardar con UpdateAsync (en orden)
    public List<Reservation> Updated { get; } = [];

    public Task<IReadOnlyList<Reservation>> GetByCourtAsync(Guid courtId)
    {
        var result = Reservations.Where(r => r.CourtId == courtId).ToList();
        return Task.FromResult<IReadOnlyList<Reservation>>(result);
    }

    public Task<Reservation?> GetByIdAsync(Guid id)
    {
        var reservation = Reservations.FirstOrDefault(r => r.Id == id);
        return Task.FromResult(reservation);
    }

    public Task AddAsync(Reservation reservation)
    {
        Reservations.Add(reservation);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Reservation reservation)
    {
        Updated.Add(reservation);
        return Task.CompletedTask;
    }
}