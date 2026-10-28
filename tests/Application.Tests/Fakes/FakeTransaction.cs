using Application.Common.Interfaces;

namespace Application.Tests.Fakes;

public class FakeTransaction : ITransaction
{
    public int RunCount { get; private set; }

    public Task<T> RunAsync<T>(Func<Task<T>> work)
    {
        RunCount++;
        return work();
    }
}