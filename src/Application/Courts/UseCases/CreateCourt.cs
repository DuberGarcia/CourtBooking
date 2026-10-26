
using Domain.Courts;

namespace Application.Courts.UseCases;

public sealed record CreateCourtCommand(string Name);

public sealed class CreateCourt
{
    private readonly ICourtRepository _courts;

    public CreateCourt(ICourtRepository courts)
    {
        _courts = courts;
    }

    public async Task<Guid> ExecuteAsync(CreateCourtCommand command)
    {
        var court = new Court(command.Name);
        await _courts.AddAsync(court);
        return court.Id;
    }
}