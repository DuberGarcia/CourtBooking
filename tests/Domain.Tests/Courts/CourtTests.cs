// tests/Domain.Tests/Courts/CourtTests.cs
using Domain.Common;
using Domain.Courts;
using Domain.Reservations;

namespace Domain.Tests.Courts;

public class CourtTests
{
    private static TimeRange Range(int startHour, int endHour) =>
        new TimeRange(new DateTime(2026, 10, 14, startHour, 0, 0),
                      new DateTime(2026, 10, 14, endHour, 0, 0));

    // ---------- Constructor ----------

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithInvalidName_ThrowsDomainException(string? name)
    {
        Assert.Throws<DomainException>(() => new Court(name!));
    }

    [Fact]
    public void Constructor_WithValidName_TrimsNameAndStartsActive()
    {
        var court = new Court("  Cancha 1  ");

        Assert.NotEqual(Guid.Empty, court.Id);
        Assert.Equal("Cancha 1", court.Name);
        Assert.True(court.IsActive);
    }

    // ---------- Deactivate ----------

    [Fact]
    public void Deactivate_WhenActive_SetsInactive()
    {
        var court = new Court("Cancha 1");

        court.Deactivate();

        Assert.False(court.IsActive);
    }

    [Fact]
    public void Deactivate_WhenAlreadyInactive_ThrowsDomainException()
    {
        var court = new Court("Cancha 1");
        court.Deactivate();

        Assert.Throws<DomainException>(() => court.Deactivate());
    }

    // ---------- Reserve ----------

    [Fact]
    public void Reserve_WithNoExistingReservations_ReturnsConfirmedReservation()
    {
        var court = new Court("Cancha 1");
        var userId = Guid.NewGuid();
        var range = Range(17, 18);

        var reservation = court.Reserve(userId, range, []);   // [] = colección vacía

        Assert.Equal(court.Id, reservation.CourtId);
        Assert.Equal(userId, reservation.UserId);
        Assert.Equal(range, reservation.TimeRange);
        Assert.Equal(ReservationStatus.Confirmada, reservation.Status);
    }

    [Fact]
    public void Reserve_WhenCourtIsInactive_ThrowsDomainException()
    {
        var court = new Court("Cancha 1");
        court.Deactivate();

        Assert.Throws<DomainException>(() => court.Reserve(Guid.NewGuid(), Range(17, 18), []));
    }

    [Fact]
    public void Reserve_WhenOverlapsConfirmedReservation_ThrowsDomainException()
    {
        var court = new Court("Cancha 1");
        var existing = court.Reserve(Guid.NewGuid(), Range(17, 19), []);

        Assert.Throws<DomainException>(() =>
            court.Reserve(Guid.NewGuid(), Range(18, 20), [existing]));
    }

    [Fact]
    public void Reserve_WhenOverlappingReservationIsCancelled_Succeeds()
    {
        var court = new Court("Cancha 1");
        var existing = court.Reserve(Guid.NewGuid(), Range(17, 19), []);
        existing.Cancelar();

        var reservation = court.Reserve(Guid.NewGuid(), Range(18, 20), [existing]);

        Assert.NotNull(reservation);
    }

    [Fact]
    public void Reserve_WhenOverlappingReservationIsFromAnotherCourt_Succeeds()
    {
        var court = new Court("Cancha 1");
        var otherCourt = new Court("Cancha 2");
        var otherReservation = otherCourt.Reserve(Guid.NewGuid(), Range(17, 19), []);

        var reservation = court.Reserve(Guid.NewGuid(), Range(18, 20), [otherReservation]);

        Assert.NotNull(reservation);
    }

    [Fact]
    public void Reserve_WhenOnlyTouchingExistingReservation_Succeeds()
    {
        var court = new Court("Cancha 1");
        var existing = court.Reserve(Guid.NewGuid(), Range(17, 18), []);

        var reservation = court.Reserve(Guid.NewGuid(), Range(18, 19), [existing]);

        Assert.NotNull(reservation);
    }

    [Fact]
    public void Reserve_WithNullExistingReservations_ThrowsArgumentNullException()
    {
        var court = new Court("Cancha 1");

        Assert.Throws<ArgumentNullException>(() => court.Reserve(Guid.NewGuid(), Range(17, 18), null!));
    }

    [Fact]
    public void Reserve_WithNullRange_ThrowsArgumentNullException()
    {
        var court = new Court("Cancha 1");

        Assert.Throws<ArgumentNullException>(() => court.Reserve(Guid.NewGuid(), null!, []));
    }
}