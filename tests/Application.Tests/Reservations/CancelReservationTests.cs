// tests/Application.Tests/Reservations/CancelReservationTests.cs
using Application.Common.Exceptions;
using Application.Reservations.UseCases;
using Application.Tests.Fakes;
using Domain.Common;
using Domain.Courts;
using Domain.Reservations;

namespace Application.Tests.Reservations;

public class CancelReservationTests
{
    private readonly InMemoryReservationRepository _reservations = new();
    private readonly CancelReservation _useCase;

    public CancelReservationTests()
    {
        _useCase = new CancelReservation(_reservations);
    }

    // El caso de uso lee el reloj real: fechas lejanas para que el resultado no dependa del día.
    private static TimeRange Future() =>
        new TimeRange(new DateTime(2030, 1, 15, 17, 0, 0), new DateTime(2030, 1, 15, 18, 0, 0));

    private static TimeRange Past() =>
        new TimeRange(new DateTime(2020, 1, 15, 17, 0, 0), new DateTime(2020, 1, 15, 18, 0, 0));

    // El constructor de Reservation es internal: se crea como en producción, con Court.Reserve.
    private Reservation SeedReservation(Guid ownerId, TimeRange range)
    {
        var court = new Court("Cancha 1");
        var reservation = court.Reserve(ownerId, range, []);
        _reservations.Reservations.Add(reservation);
        return reservation;
    }

    [Fact]
    public async Task Execute_ByOwner_CancelsAndSaves()
    {
        var ownerId = Guid.NewGuid();
        var reservation = SeedReservation(ownerId, Future());

        await _useCase.ExecuteAsync(new CancelReservationCommand(reservation.Id, ownerId, false));

        Assert.Equal(ReservationStatus.Cancelada, reservation.Status);
        var saved = Assert.Single(_reservations.Updated);
        Assert.Same(reservation, saved);   // Same = el MISMO objeto, no solo uno igual
    }

    [Fact]
    public async Task Execute_ByAdminWhoIsNotOwner_CancelsAndSaves()
    {
        var reservation = SeedReservation(Guid.NewGuid(), Future());
        var adminId = Guid.NewGuid();

        await _useCase.ExecuteAsync(new CancelReservationCommand(reservation.Id, adminId, true));

        Assert.Equal(ReservationStatus.Cancelada, reservation.Status);
        Assert.Single(_reservations.Updated);
    }

    [Fact]
    public async Task Execute_ByAnotherCustomer_ThrowsForbiddenAndChangesNothing()
    {
        var reservation = SeedReservation(Guid.NewGuid(), Future());
        var strangerId = Guid.NewGuid();

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _useCase.ExecuteAsync(new CancelReservationCommand(reservation.Id, strangerId, false)));

        Assert.Equal(ReservationStatus.Confirmada, reservation.Status);
        Assert.Empty(_reservations.Updated);
    }

    [Fact]
    public async Task Execute_ByAnotherCustomerOnCancelledReservation_ThrowsForbiddenNotDomain()
    {
        // Un extraño no debe enterarse de que la reserva ya estaba cancelada:
        // la autorización va antes que las reglas del dominio.
        var reservation = SeedReservation(Guid.NewGuid(), Future());
        reservation.Cancelar(new DateTime(2030, 1, 1));

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _useCase.ExecuteAsync(new CancelReservationCommand(reservation.Id, Guid.NewGuid(), false)));
    }

    [Fact]
    public async Task Execute_WhenReservationDoesNotExist_ThrowsNotFound()
    {
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _useCase.ExecuteAsync(new CancelReservationCommand(Guid.NewGuid(), Guid.NewGuid(), true)));

        Assert.Empty(_reservations.Updated);
    }

    [Fact]
    public async Task Execute_WhenAlreadyStarted_ThrowsDomainExceptionAndSavesNothing()
    {
        var ownerId = Guid.NewGuid();
        var reservation = SeedReservation(ownerId, Past());

        await Assert.ThrowsAsync<DomainException>(() =>
            _useCase.ExecuteAsync(new CancelReservationCommand(reservation.Id, ownerId, false)));

        Assert.Equal(ReservationStatus.Confirmada, reservation.Status);
        Assert.Empty(_reservations.Updated);
    }

    [Fact]
    public async Task Execute_ByAdminWhenAlreadyStarted_AlsoThrowsDomainException()
    {
        // Decisión: la regla de tiempo vive en el Domain, que no conoce roles → aplica también al Admin.
        var reservation = SeedReservation(Guid.NewGuid(), Past());

        await Assert.ThrowsAsync<DomainException>(() =>
            _useCase.ExecuteAsync(new CancelReservationCommand(reservation.Id, Guid.NewGuid(), true)));

        Assert.Empty(_reservations.Updated);
    }

    [Fact]
    public async Task Execute_WhenAlreadyCancelled_ThrowsDomainExceptionAndSavesNothing()
    {
        var ownerId = Guid.NewGuid();
        var reservation = SeedReservation(ownerId, Future());
        reservation.Cancelar(new DateTime(2030, 1, 1));

        await Assert.ThrowsAsync<DomainException>(() =>
            _useCase.ExecuteAsync(new CancelReservationCommand(reservation.Id, ownerId, false)));

        Assert.Empty(_reservations.Updated);
    }
}