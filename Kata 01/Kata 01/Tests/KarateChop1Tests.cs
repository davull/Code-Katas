using Xunit;
using Xunit.Abstractions;

namespace Kata_01.Tests;

public class KarateChop1Tests
{
    private readonly ITestOutputHelper _output;

    public KarateChop1Tests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void Test_chop(int[] arr, int search, int expected)
    {
        // Arrange
        var sut = new KarateChop1();

        // Act
        var actual = sut.Chop(arr: arr, search: search);

        _output.WriteLine($"arr: {string.Join(",", arr)}, " +
                          $"search: {search}, " +
                          $"expected: {expected}, " +
                          $"actual: {actual}");

        // Assert
        Assert.Equal(expected, actual);
    }
}