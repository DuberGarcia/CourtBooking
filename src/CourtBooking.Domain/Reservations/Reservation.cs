namespace CourtBooking.Domain.Reservations;

public class Reservation
{
    public Guid Id { get; }
    public Guid CourtId { get; }
    public Guid UserId { get; }
    public TimeRange Period { get; }
    public ReservationStatus Status { get; private set; }  

    // Solo para EF Core. Es privado
    private Reservation()
    {
        Period = null!;
    }
    internal Reservation(Guid courtId, Guid userId, TimeRange period)
    {
        if(courtId == Guid.Empty) throw new ArgumentException("El Id de la cancha no es valido");
        if(userId == Guid.Empty) throw new ArgumentException("El Id del usuario no es valido");
        ArgumentNullException.ThrowIfNull(period);

        Id = Guid.NewGuid();
        CourtId = courtId;
        UserId = userId;
        Period = period;
        Status = ReservationStatus.Confirmed;
    }

    public void Cancel()
    {
        if(Status == ReservationStatus.Cancelled) throw new InvalidOperationException("La reservacion ya esta cancelada");
        Status = ReservationStatus.Cancelled;
    }
}