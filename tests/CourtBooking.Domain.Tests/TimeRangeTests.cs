using CourtBooking.Domain.Reservations;

namespace CourtBooking.Domain.Tests;

public class TimeRangeTests
{
    private static DateTime At(int hour, int minute = 0) =>
        new(2026, 10, 1, hour, minute, 0);

    [Fact]
    public void Crea_un_intervalo_valido()
    {
        var range = new TimeRange(At(17), At(18));

        Assert.Equal(At(17), range.Start);
        Assert.Equal(At(18), range.End);
    }

    [Fact]
    public void No_permite_inicio_despues_del_fin()
    {
        Assert.Throws<ArgumentException>(() => new TimeRange(At(18), At(17)));
    }

    [Fact]
    public void No_permite_inicio_igual_al_fin()
    {
        Assert.Throws<ArgumentException>(() => new TimeRange(At(17), At(17)));
    }

    [Fact]
    public void No_permite_duracion_menor_a_una_hora()
    {
        Assert.Throws<ArgumentException>(() => new TimeRange(At(17), At(17, 30)));
    }

    [Fact]
    public void Dos_intervalos_con_los_mismos_valores_son_iguales()
    {
        var a = new TimeRange(At(17), At(18));
        var b = new TimeRange(At(17), At(18));

        Assert.Equal(a, b);
    }

    [Theory]
    [InlineData(17, 19, 18, 20, true)]   // se cruzan parcialmente
    [InlineData(17, 20, 18, 19, true)]   // uno contiene al otro
    [InlineData(17, 18, 18, 19, false)]  // contiguos: NO chocan
    [InlineData(17, 18, 19, 20, false)]  // separados
    public void Detecta_solapamiento(int s1, int e1, int s2, int e2, bool esperado)
    {
        var a = new TimeRange(At(s1), At(e1));
        var b = new TimeRange(At(s2), At(e2));

        Assert.Equal(esperado, a.Overlaps(b));
        Assert.Equal(esperado, b.Overlaps(a)); // debe funcionar en ambos sentidos
    }
}