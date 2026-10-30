using Application.Common.Exceptions;
using Application.Reservations.Models;
using Domain.Common;
using Domain.Courts;
using Domain.Reservations;

namespace Application.Reservations.UseCases;
public sealed record GetAvailabilityQuery(Guid CourtId, DateOnly Date);
public class GetAvailability
{
    private readonly ICourtRepository _courts;
    private readonly IReservationRepository _reservations;

    public GetAvailability(ICourtRepository courts, IReservationRepository reservations)
    {
        _courts = courts;
        _reservations = reservations;
    }

    public async Task<CourtAvailability> ExecuteAsync(GetAvailabilityQuery command)
    {
        var court = await _courts.GetByIdAsync(command.CourtId);
        if (court is null)
            throw new NotFoundException("No se encontro la cancha");

        var dayStart = command.Date.ToDateTime(TimeOnly.MinValue);
        var range = new TimeRange(dayStart,dayStart.AddDays(1));

        var reservations = await _reservations.GetConfirmedOverlappingAsync(court.Id, range);

        var OccupiedSlots = reservations
            .OrderBy(r => r.TimeRange.Start)
            .Select(r => new OccupiedSlot(r.TimeRange.Start, r.TimeRange.End))
            .ToList();
        
        return new CourtAvailability(court.Id,command.Date,OccupiedSlots);
    }
}