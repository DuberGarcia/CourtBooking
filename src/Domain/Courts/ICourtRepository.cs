namespace Domain.Courts;

public interface ICourtRepository
{
    Task<Court?> GetByIdAsync(Guid id);
}