// tests/Application.Tests/Fakes/InMemoryCourtRepository.cs
using Domain.Courts;

namespace Application.Tests.Fakes;

public class InMemoryCourtRepository : ICourtRepository
{
    public List<Court> Courts { get; } = [];

    public Task<Court?> GetByIdAsync(Guid id)
    {
        var court = Courts.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(court);
    }
}