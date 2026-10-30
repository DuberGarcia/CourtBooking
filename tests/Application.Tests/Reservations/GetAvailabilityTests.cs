// tests/Application.Tests/Reservations/GetAvailabilityTests.cs
using Application.Common.Exceptions;
using Application.Reservations.Models;
using Application.Reservations.UseCases;
using Application.Tests.Fakes;
using Domain.Common;
using Domain.Courts;

namespace Application.Tests.Reservations;

public class GetAvailabilityTests
{
    private readonly InMemoryCourtRepository _courts = new();
    private readonly InMemoryReservationRepository _reservations = new();
    private readonly GetAvailability _useCase;

    // El día que se consulta en todos los tests: 2 dic 2026
    private static readonly DateOnly Day = new(2026, 12, 2);

    public GetAvailabilityTests()
    {
        _useCase = new GetAvailability(_courts, _reservations);
    }

    // 2 dic 2026 a la hora indicada; dayOffset mueve el día (-1 = 1 dic, +1 = 3 dic)
    private static DateTime At(int hour, int dayOffset = 0) =>
        new DateTime(2026, 12, 2, hour, 0, 0).AddDays(dayOffset);

    private Court SeedCourt()
    {
        var court = new Court("Cancha 1");
        _courts.Courts.Add(court);
        return court;
    }

    // Las reservas se crean con Court.Reserve (el constructor de Reservation es internal)
    private void SeedReservation(Court court, DateTime start, DateTime end)
    {
        var reservation = court.Reserve(Guid.NewGuid(), new TimeRange(start, end), []);
        _reservations.Reservations.Add(reservation);
    }

    [Fact]
    public async Task Execute_WhenCourtDoesNotExist_ThrowsNotFoundException()
    {
        var query = new GetAvailabilityQuery(Guid.NewGuid(), Day);

        await Assert.ThrowsAsync<NotFoundException>(() => _useCase.ExecuteAsync(query));
    }

    [Fact]
    public async Task Execute_WithNoReservations_ReturnsCourtDateAndEmptyList()
    {
        var court = SeedCourt();

        var result = await _useCase.ExecuteAsync(new GetAvailabilityQuery(court.Id, Day));

        Assert.Equal(court.Id, result.CourtId);
        Assert.Equal(Day, result.Date);
        Assert.Empty(result.OccupiedSlots);
    }

    [Fact]
    public async Task Execute_ReturnsOnlyThatDay_OrderedByStart()
    {
        var court = SeedCourt();
        SeedReservation(court, At(17), At(18));
        SeedReservation(court, At(8), At(9));              // se agrega después, pero empieza antes
        SeedReservation(court, At(17, -1), At(18, -1));    // 1 dic: otro día
        SeedReservation(court, At(0, 1), At(1, 1));        // 3 dic 00:00: solo toca el límite del día

        var result = await _useCase.ExecuteAsync(new GetAvailabilityQuery(court.Id, Day));

        OccupiedSlot[] expected =
        [
            new(At(8), At(9)),
            new(At(17), At(18)),
        ];
        Assert.Equal(expected, result.OccupiedSlots);
    }

    [Fact]
    public async Task Execute_IncludesReservationThatCrossesMidnight_WithItsRealTimes()
    {
        var court = SeedCourt();
        SeedReservation(court, At(23, -1), At(1));         // 1 dic 23:00 → 2 dic 01:00

        var result = await _useCase.ExecuteAsync(new GetAvailabilityQuery(court.Id, Day));

        var slot = Assert.Single(result.OccupiedSlots);
        Assert.Equal(new OccupiedSlot(At(23, -1), At(1)), slot);
    }
}