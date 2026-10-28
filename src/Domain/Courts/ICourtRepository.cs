namespace Domain.Courts;

public interface ICourtRepository
{
    Task<Court?> GetByIdAsync(Guid id);
    Task<Court?> GetByIdForUpdateAsync(Guid id);
    Task AddAsync(Court court);
}