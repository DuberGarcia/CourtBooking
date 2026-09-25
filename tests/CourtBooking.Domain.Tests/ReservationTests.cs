using CourtBooking.Domain.Reservations;

namespace CourtBooking.Domain.Tests;

public class ReservationTests
{
    private static TimeRange UnaHora() =>
        new(new DateTime(2026, 10, 1, 17, 0, 0), new DateTime(2026, 10, 1, 18, 0, 0));

    [Fact]
    public void Una_reserva_nueva_queda_confirmada_y_con_id()
    {
        var reserva = new Reservation(Guid.NewGuid(), Guid.NewGuid(), UnaHora());

        Assert.NotEqual(Guid.Empty, reserva.Id);
        Assert.Equal(ReservationStatus.Confirmed, reserva.Status);
    }

    [Fact]
    public void No_permite_cancha_vacia()
    {
        Assert.Throws<ArgumentException>(() =>
            new Reservation(Guid.Empty, Guid.NewGuid(), UnaHora()));
    }

    [Fact]
    public void No_permite_usuario_vacio()
    {
        Assert.Throws<ArgumentException>(() =>
            new Reservation(Guid.NewGuid(), Guid.Empty, UnaHora()));
    }

    [Fact]
    public void No_permite_intervalo_nulo()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new Reservation(Guid.NewGuid(), Guid.NewGuid(), null!));
    }

    [Fact]
    public void Cancelar_cambia_el_estado_pero_no_la_identidad()
    {
        var reserva = new Reservation(Guid.NewGuid(), Guid.NewGuid(), UnaHora());
        var idOriginal = reserva.Id;

        reserva.Cancel();

        Assert.Equal(ReservationStatus.Cancelled, reserva.Status);
        Assert.Equal(idOriginal, reserva.Id);
    }

    [Fact]
    public void No_se_puede_cancelar_dos_veces()
    {
        var reserva = new Reservation(Guid.NewGuid(), Guid.NewGuid(), UnaHora());
        reserva.Cancel();

        Assert.Throws<InvalidOperationException>(() => reserva.Cancel());
    }
}