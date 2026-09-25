namespace CourtBooking.Domain.Reservations;

public record TimeRange
{
    public DateTime Start { get; }
    public DateTime End { get; }

    public TimeRange(DateTime start, DateTime end)
    {
        if(start >= end) throw new ArgumentException("La Hora de inicio no puede ser igual o mayor a la de final");
        TimeSpan duracion = end - start;
        if(duracion < TimeSpan.FromHours(1)) throw new ArgumentException("La duracion minima es de 1 hora");
        Start = start;
        End = end;
    }

    public bool Overlaps(TimeRange other)
    {   
        bool thisTerminaAntes = End <= other.Start;
        bool otherTerminaAntes = other.End <= Start;
        bool noChocan = thisTerminaAntes || otherTerminaAntes;
        return !noChocan;
    }
}