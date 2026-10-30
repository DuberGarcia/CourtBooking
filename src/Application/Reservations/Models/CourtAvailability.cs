namespace Application.Reservations.Models;

public sealed record OccupiedSlot(DateTime Start, DateTime End);
public sealed record CourtAvailability(Guid CourtId, DateOnly Date, IReadOnlyList<OccupiedSlot> OccupiedSlots);