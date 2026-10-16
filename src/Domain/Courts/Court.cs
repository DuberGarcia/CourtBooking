using Domain.Common;
using Domain.Reservations;

namespace Domain.Courts;
public class Court
{
    public Guid Id { get; }
    public string Name { get; }
    public bool IsActive { get; private set; }

    private Court()
    {
        Name = null!;
    }
    public Court(string name)
    {
        if(string.IsNullOrWhiteSpace(name)) throw new DomainException("El Nombre de la cancha es obligatorio");
        
        Id = Guid.NewGuid();
        Name=name.Trim();
        IsActive=true;
    }

    public void Deactivate()
    {
        if(!IsActive) throw new DomainException("La cancha ya esta desactivada");
        IsActive = false;
    }

    public Reservation Reserve(Guid userId,TimeRange range,IEnumerable<Reservation> existingReservations)
    {
        ArgumentNullException.ThrowIfNull(existingReservations);

        if(!IsActive) throw new DomainException("La cancha no esta activa");
        
        var IsTaken = existingReservations.Any( res => 
            res.CourtId == Id &&
            range.Overlaps(res.TimeRange) && 
            res.Status == ReservationStatus.Confirmada);

        if(IsTaken) throw new DomainException("La cancha no esta disponible");

        return new Reservation(Id,userId,range);
    }
}