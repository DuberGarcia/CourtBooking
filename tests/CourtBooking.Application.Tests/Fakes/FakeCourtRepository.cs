using CourtBooking.Domain.Courts;

namespace CourtBooking.Application.Tests.Fakes;

public class FakeCourtRepository : ICourtRepository   // ": ICourtRepository" = implementa la interfaz
{
    private readonly List<Court> _courts = [];

    // No está en la interfaz: es solo para preparar los tests
    public void Add(Court court) => _courts.Add(court);

    public Task AddAsync(Court court)
    {
        _courts.Add(court);
        return Task.CompletedTask;
    }

    public Task<Court?> GetByIdAsync(Guid id)
    {
        var court = _courts.FirstOrDefault(c => c.Id == id);   // null si no la encuentra
        return Task.FromResult(court);
    }
}