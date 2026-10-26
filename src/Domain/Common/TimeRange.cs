
namespace Domain.Common;

public sealed record TimeRange
{
    public DateTime Start { get; }
    public DateTime End { get; }
    public TimeSpan Duration => End - Start;

    public TimeRange(DateTime start, DateTime end)
    {
        if (start >= end) throw new DomainException("El inicio no puede ser mayor o igual al final");
        Start = start;
        End = end;
    }

    public bool Overlaps(TimeRange Other)
    {
        if (End <= Other.Start) return false;
        if (Other.End <= Start) return false;
        return true;
    }
}