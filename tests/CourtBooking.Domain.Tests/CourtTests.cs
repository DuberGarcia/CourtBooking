using CourtBooking.Domain.Courts;
using CourtBooking.Domain.Reservations;

namespace CourtBooking.Domain.Tests;

public class CourtTests
{
    [Fact]
    public void Una_cancha_nueva_queda_activa_con_id_y_nombre()
    {
        var cancha = new Court("Cancha 1");

        Assert.NotEqual(Guid.Empty, cancha.Id);
        Assert.Equal("Cancha 1", cancha.Name);
        Assert.True(cancha.IsActive);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void No_permite_nombre_vacio(string? nombre)
    {
        Assert.Throws<ArgumentException>(() => new Court(nombre!));
    }

    [Fact]
    public void No_permite_nombre_de_mas_de_50_caracteres()
    {
        var nombreLargo = new string('a', 51);

        Assert.Throws<ArgumentException>(() => new Court(nombreLargo));
    }

    [Fact]
    public void Guarda_el_nombre_sin_espacios_al_inicio_ni_al_final()
    {
        var cancha = new Court("   Cancha 1   ");

        Assert.Equal("Cancha 1", cancha.Name);
    }

    [Fact]
    public void Desactivar_deja_la_cancha_inactiva()
    {
        var cancha = new Court("Cancha 1");

        cancha.Deactivate();

        Assert.False(cancha.IsActive);
    }

    [Fact]
    public void No_se_puede_desactivar_dos_veces()
    {
        var cancha = new Court("Cancha 1");
        cancha.Deactivate();

        Assert.Throws<InvalidOperationException>(() => cancha.Deactivate());
    }

    [Fact]
    public void Activar_una_cancha_inactiva()
    {
        var cancha = new Court("Cancha 1");
        cancha.Deactivate();

        cancha.Activate();

        Assert.True(cancha.IsActive);
    }

    [Fact]
    public void No_se_puede_activar_una_cancha_activa()
    {
        var cancha = new Court("Cancha 1");

        Assert.Throws<InvalidOperationException>(() => cancha.Activate());
    }

    private static TimeRange UnaHora() =>
        new(new DateTime(2026, 10, 1, 17, 0, 0), new DateTime(2026, 10, 1, 18, 0, 0));

    [Fact]
    public void Reservar_una_cancha_activa_crea_una_reserva_confirmada()
    {
        var cancha = new Court("Cancha 1");
        var userId = Guid.NewGuid();

        var reserva = cancha.Reserve(userId, UnaHora(),[]);

        Assert.Equal(cancha.Id, reserva.CourtId);
        Assert.Equal(userId, reserva.UserId);
        Assert.Equal(ReservationStatus.Confirmed, reserva.Status);
    }

    [Fact]
    public void No_se_puede_reservar_una_cancha_inactiva()
    {
        var cancha = new Court("Cancha 1");
        cancha.Deactivate();

        Assert.Throws<InvalidOperationException>(() => cancha.Reserve(Guid.NewGuid(), UnaHora(),[]));
    }

    private static DateTime At(int hour) => new(2026, 10, 1, hour, 0, 0);

    [Fact]
    public void No_se_puede_reservar_si_choca_con_una_reserva_confirmada()
    {
        var cancha = new Court("Cancha 1");
        var existente = cancha.Reserve(Guid.NewGuid(), new TimeRange(At(17), At(19)), []);

        Assert.Throws<InvalidOperationException>(() =>
            cancha.Reserve(Guid.NewGuid(), new TimeRange(At(18), At(20)), [existente]));
    }

    [Fact]
    public void Se_puede_reservar_si_choca_solo_con_una_reserva_cancelada()
    {
        var cancha = new Court("Cancha 1");
        var cancelada = cancha.Reserve(Guid.NewGuid(), new TimeRange(At(17), At(19)), []);
        cancelada.Cancel();

        var nueva = cancha.Reserve(Guid.NewGuid(), new TimeRange(At(18), At(20)), [cancelada]);

        Assert.Equal(ReservationStatus.Confirmed, nueva.Status);
    }

    [Fact]
    public void Se_puede_reservar_un_horario_contiguo()
    {
        var cancha = new Court("Cancha 1");
        var existente = cancha.Reserve(Guid.NewGuid(), new TimeRange(At(17), At(18)), []);

        var nueva = cancha.Reserve(Guid.NewGuid(), new TimeRange(At(18), At(19)), [existente]);

        Assert.Equal(ReservationStatus.Confirmed, nueva.Status);
    }
}