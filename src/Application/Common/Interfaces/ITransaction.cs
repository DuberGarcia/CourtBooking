namespace Application.Common.Interfaces;
public interface ITransaction
{
    Task<T> RunAsync<T>(Func<Task<T>> work);
}