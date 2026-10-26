using Domain.Common;

namespace Domain.Reservations;
public class Reservation
{
    public Guid Id { get; }
    public Guid CourtId { get; }
    public Guid UserId { get; }
    public TimeRange TimeRange { get; }
    public ReservationStatus Status { get; private set; }

    private Reservation()
    {
        TimeRange = null!;
    }

    internal Reservation(Guid courtId, Guid userId, TimeRange range)
    {
        if (courtId == Guid.Empty) throw new DomainException("Id de la cancha no es valido");
        if (userId == Guid.Empty) throw new DomainException("Id del usuario no es valido");

        ArgumentNullException.ThrowIfNull(range);

        Id = Guid.NewGuid();
        CourtId = courtId;
        UserId = userId;
        TimeRange = range;
        Status = ReservationStatus.Confirmada;
    }
    public void Cancelar()
    {
        if (Status == ReservationStatus.Cancelada)
            throw new DomainException("La reserva ya fue cancelada.");

        Status = ReservationStatus.Cancelada;
    }
}