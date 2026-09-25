using CourtBooking.Domain.Reservations;

namespace CourtBooking.Domain.Courts;

public class Court
{
    public Guid Id { get; }
    public string Name { get; private set; }
    public bool IsActive { get; private set;}

    public Court(string name)
    {
        if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("El nombre es requerido");
        string trimmedName = name.Trim();
        if(trimmedName.Length > 50) throw new ArgumentException("El nombre no debe tener mas de 50 caracteres");
        Id = Guid.NewGuid();
        Name = trimmedName;
        IsActive = true;
    }

    public void Deactivate()
    {
        if(!IsActive) throw new InvalidOperationException("La cancha ya esta inactiva");
        IsActive = false;
    }

    public void Activate()
    {
        if(IsActive) throw new InvalidOperationException("La cancha ya esta activa");
        IsActive = true;
    }
    public Reservation Reserve(Guid userId, TimeRange period,IEnumerable<Reservation> existingReservations)
    {
        if(!IsActive) throw new InvalidOperationException("La cancha esta inactiva");

        bool hayChoque = existingReservations.Any(r => 
            r.Period.Overlaps(period) && 
            r.CourtId == Id && 
            r.Status == ReservationStatus.Confirmed);

        if(hayChoque) throw new InvalidOperationException("La cancha ya esta reservada");

        return new Reservation(Id, userId, period);
    }
}