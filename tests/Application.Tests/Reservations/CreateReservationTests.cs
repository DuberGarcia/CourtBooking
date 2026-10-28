// tests/Application.Tests/Reservations/CreateReservationTests.cs
using Application.Common.Exceptions;
using Application.Reservations.UseCases;
using Application.Tests.Fakes;
using Domain.Common;
using Domain.Courts;
using Domain.Reservations;

namespace Application.Tests.Reservations;

public class CreateReservationTests
{
    private readonly InMemoryCourtRepository _courts = new();
    private readonly InMemoryReservationRepository _reservations = new();
    private readonly CreateReservation _useCase;
    private readonly FakeTransaction _transaction = new();

    // xUnit crea una instancia nueva de la clase por cada test:
    // cada test arranca con repositorios vacíos.
    public CreateReservationTests()
    {
        _useCase = new CreateReservation(_courts, _reservations, _transaction);
    }

    private static DateTime At(int hour) => new DateTime(2026, 10, 14, hour, 0, 0);

    private Court SeedCourt()
    {
        var court = new Court("Cancha 1");
        _courts.Courts.Add(court);
        return court;
    }

    [Fact]
    public async Task Execute_WithValidData_SavesReservationAndReturnsItsId()
    {
        var court = SeedCourt();
        var userId = Guid.NewGuid();
        var command = new CreateReservationCommand(court.Id, userId, At(17), At(18));

        var id = await _useCase.ExecuteAsync(command);

        var saved = Assert.Single(_reservations.Reservations);   // exactamente una guardada
        Assert.Equal(id, saved.Id);
        Assert.Equal(court.Id, saved.CourtId);
        Assert.Equal(userId, saved.UserId);
        Assert.Equal(ReservationStatus.Confirmada, saved.Status);
    }

    [Fact]
    public async Task Execute_WhenCourtDoesNotExist_ThrowsNotFoundException()
    {
        var command = new CreateReservationCommand(Guid.NewGuid(), Guid.NewGuid(), At(17), At(18));

        await Assert.ThrowsAsync<NotFoundException>(() => _useCase.ExecuteAsync(command));
        Assert.Empty(_reservations.Reservations);
    }

    [Fact]
    public async Task Execute_WithInvalidRange_ThrowsDomainExceptionAndSavesNothing()
    {
        var court = SeedCourt();
        var command = new CreateReservationCommand(court.Id, Guid.NewGuid(), At(18), At(17));

        await Assert.ThrowsAsync<DomainException>(() => _useCase.ExecuteAsync(command));
        Assert.Empty(_reservations.Reservations);
    }

    [Fact]
    public async Task Execute_WhenCourtIsInactive_ThrowsDomainExceptionAndSavesNothing()
    {
        var court = SeedCourt();
        court.Deactivate();
        var command = new CreateReservationCommand(court.Id, Guid.NewGuid(), At(17), At(18));

        await Assert.ThrowsAsync<DomainException>(() => _useCase.ExecuteAsync(command));
        Assert.Empty(_reservations.Reservations);
    }

    [Fact]
    public async Task Execute_WhenOverlapsExistingReservation_ThrowsDomainException()
    {
        var court = SeedCourt();
        var existing = court.Reserve(Guid.NewGuid(), new TimeRange(At(17), At(19)), []);
        _reservations.Reservations.Add(existing);

        var command = new CreateReservationCommand(court.Id, Guid.NewGuid(), At(18), At(20));

        await Assert.ThrowsAsync<DomainException>(() => _useCase.ExecuteAsync(command));
        Assert.Single(_reservations.Reservations);   // solo la que ya existía
    }
    
    [Fact]
    public async Task Execute_RunsInsideATransactionAndLocksTheCourt()
    {
        var court = SeedCourt();
        var command = new CreateReservationCommand(court.Id, Guid.NewGuid(), At(17), At(18));

        await _useCase.ExecuteAsync(command);

        Assert.Equal(1, _transaction.RunCount);     // se ejecutó dentro de la transacción
        Assert.Equal(1, _courts.ForUpdateCalls);    // leyó la cancha con bloqueo
    }
}