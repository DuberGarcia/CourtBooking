// tests/Domain.Tests/Common/TimeRangeTests.cs
using Domain.Common;

namespace Domain.Tests.Common;

public class TimeRangeTests
{
    private static DateTime At(int hour) => new DateTime(2026, 10, 13, hour, 0, 0);

    [Fact]
    public void Constructor_WhenStartIsAfterEnd_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => new TimeRange(At(18), At(17)));
    }

    [Fact]
    public void Constructor_WhenStartEqualsEnd_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => new TimeRange(At(17), At(17)));
    }

    [Fact]
    public void Constructor_WithValidRange_AssignsStartAndEnd()
    {
        var range = new TimeRange(At(17), At(18));

        Assert.Equal(At(17), range.Start);
        Assert.Equal(At(18), range.End);
    }

    [Fact]
    public void Duration_IsDifferenceBetweenEndAndStart()
    {
        var range = new TimeRange(At(17), At(19));

        Assert.Equal(TimeSpan.FromHours(2), range.Duration);
    }

    [Fact]
    public void TwoRanges_WithSameStartAndEnd_AreEqual()
    {
        var a = new TimeRange(At(17), At(18));
        var b = new TimeRange(At(17), At(18));

        Assert.Equal(a, b);
    }

    [Theory]
    [InlineData(17, 19, 18, 20, true)]   // cruce parcial
    [InlineData(17, 21, 18, 19, true)]   // uno dentro del otro
    [InlineData(17, 18, 17, 18, true)]   // idénticos
    [InlineData(17, 19, 18, 19, true)]   // terminan igual
    [InlineData(17, 18, 18, 19, false)]  // solo se tocan
    [InlineData(17, 18, 20, 21, false)]  // separados, A antes
    [InlineData(20, 21, 17, 18, false)]  // separados, B antes
    public void Overlaps_ReturnsExpectedResult(int aStart, int aEnd, int bStart, int bEnd, bool expected)
    {
        var a = new TimeRange(At(aStart), At(aEnd));
        var b = new TimeRange(At(bStart), At(bEnd));

        Assert.Equal(expected, a.Overlaps(b));
        Assert.Equal(expected, b.Overlaps(a));   // el solapamiento debe ser simétrico
    }
}