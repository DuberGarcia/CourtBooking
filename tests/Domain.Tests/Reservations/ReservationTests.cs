// tests/Domain.Tests/Reservations/ReservationTests.cs
using Domain.Common;
using Domain.Reservations;

namespace Domain.Tests.Reservations;

public class ReservationTests
{
    private static TimeRange ValidRange() =>
        new TimeRange(new DateTime(2026, 10, 13, 17, 0, 0), new DateTime(2026, 10, 13, 18, 0, 0));

    private static Reservation ValidReservation() =>
        new Reservation(Guid.NewGuid(), Guid.NewGuid(), ValidRange());

    [Fact]
    public void Constructor_WithEmptyCourtId_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => new Reservation(Guid.Empty, Guid.NewGuid(), ValidRange()));
    }

    [Fact]
    public void Constructor_WithEmptyUserId_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => new Reservation(Guid.NewGuid(), Guid.Empty, ValidRange()));
    }

    [Fact]
    public void Constructor_WithNullRange_ThrowsArgumentNullException()
    {
        // "null!" le dice al compilador: "sé que esto es null, déjame pasarlo a propósito"
        Assert.Throws<ArgumentNullException>(() => new Reservation(Guid.NewGuid(), Guid.NewGuid(), null!));
    }

    [Fact]
    public void Constructor_WithValidData_AssignsValuesAndStartsConfirmed()
    {
        var courtId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var range = ValidRange();

        var reservation = new Reservation(courtId, userId, range);

        Assert.NotEqual(Guid.Empty, reservation.Id);
        Assert.Equal(courtId, reservation.CourtId);
        Assert.Equal(userId, reservation.UserId);
        Assert.Equal(range, reservation.TimeRange);
        Assert.Equal(ReservationStatus.Confirmada, reservation.Status);
    }

    [Fact]
    public void TwoReservations_WithSameData_HaveDifferentIds()
    {
        var courtId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var a = new Reservation(courtId, userId, ValidRange());
        var b = new Reservation(courtId, userId, ValidRange());

        Assert.NotEqual(a.Id, b.Id);   // misma información, distinta identidad
    }

    [Fact]
    public void Cancelar_WhenConfirmed_ChangesStatusToCancelada()
    {
        var reservation = ValidReservation();

        reservation.Cancelar(new DateTime(2026, 10, 13, 16, 0, 0));

        Assert.Equal(ReservationStatus.Cancelada, reservation.Status);
    }

    [Fact]
    public void Cancelar_WhenAlreadyCancelled_ThrowsDomainException()
    {
        var reservation = ValidReservation();
        reservation.Cancelar(new DateTime(2026, 10, 13, 16, 0, 0));

        Assert.Throws<DomainException>(() => reservation.Cancelar(new DateTime(2026, 10, 13, 16, 0, 0)));
    }
        // ValidRange() es 13 oct, 17:00–18:00

    [Fact]
    public void Cancelar_OneMinuteBeforeStart_ChangesStatusToCancelada()
    {
        var reservation = ValidReservation();

        reservation.Cancelar(new DateTime(2026, 10, 13, 16, 59, 0));

        Assert.Equal(ReservationStatus.Cancelada, reservation.Status);
    }

    [Theory]
    [InlineData(17, 0)]    // justo a la hora de inicio
    [InlineData(17, 30)]   // en curso
    [InlineData(19, 0)]    // ya terminó
    public void Cancelar_WhenAlreadyStarted_ThrowsAndStaysConfirmed(int hour, int minute)
    {
        var reservation = ValidReservation();
        var now = new DateTime(2026, 10, 13, hour, minute, 0);

        Assert.Throws<DomainException>(() => reservation.Cancelar(now));
        Assert.Equal(ReservationStatus.Confirmada, reservation.Status);   // no cambió nada
    }
}
