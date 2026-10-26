// tests/Application.Tests/Courts/CreateCourtTests.cs
using Application.Courts.UseCases;
using Application.Tests.Fakes;
using Domain.Common;

namespace Application.Tests.Courts;

public class CreateCourtTests
{
    private readonly InMemoryCourtRepository _courts = new();
    private readonly CreateCourt _useCase;

    public CreateCourtTests()
    {
        _useCase = new CreateCourt(_courts);
    }

    [Fact]
    public async Task Execute_WithValidName_SavesActiveCourtAndReturnsItsId()
    {
        var id = await _useCase.ExecuteAsync(new CreateCourtCommand("  Cancha 1  "));

        var saved = Assert.Single(_courts.Courts);
        Assert.Equal(id, saved.Id);
        Assert.Equal("Cancha 1", saved.Name);
        Assert.True(saved.IsActive);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Execute_WithInvalidName_ThrowsDomainExceptionAndSavesNothing(string name)
    {
        await Assert.ThrowsAsync<DomainException>(() =>
            _useCase.ExecuteAsync(new CreateCourtCommand(name)));

        Assert.Empty(_courts.Courts);
    }
}