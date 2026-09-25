using CourtBooking.Application.Reservations;
using CourtBooking.Application.Tests.Fakes;
using CourtBooking.Domain.Courts;

namespace CourtBooking.Application.Tests;

public class CreateReservationTests
{
    private static DateTime At(int hour) => new(2026, 10, 1, hour, 0, 0);

    private readonly FakeCourtRepository _courts = new();
    private readonly FakeReservationRepository _reservations = new();
    private readonly CreateReservation _useCase;

    // xUnit crea una instancia nueva de esta clase POR CADA test,
    // así que cada test empieza con repositorios vacíos.
    public CreateReservationTests()
    {
        _useCase = new CreateReservation(_courts, _reservations);
    }

    [Fact]
    public async Task Crea_y_guarda_la_reserva()
    {
        var cancha = new Court("Cancha 1");
        _courts.Add(cancha);

        var id = await _useCase.ExecuteAsync(cancha.Id, Guid.NewGuid(), At(17), At(18));

        var guardada = Assert.Single(_reservations.Saved);   // exactamente una
        Assert.Equal(id, guardada.Id);
    }

    [Fact]
    public async Task Falla_si_la_cancha_no_existe()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _useCase.ExecuteAsync(Guid.NewGuid(), Guid.NewGuid(), At(17), At(18)));
    }

    [Fact]
    public async Task No_guarda_nada_si_hay_choque()
    {
        var cancha = new Court("Cancha 1");
        _courts.Add(cancha);
        await _useCase.ExecuteAsync(cancha.Id, Guid.NewGuid(), At(17), At(19));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _useCase.ExecuteAsync(cancha.Id, Guid.NewGuid(), At(18), At(20)));

        Assert.Single(_reservations.Saved);   // solo la primera
    }
}