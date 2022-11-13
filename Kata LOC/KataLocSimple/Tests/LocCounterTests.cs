using FluentAssertions;
using KataLocSimple.Lib;
using Xunit;

namespace KataLocSimple.Tests;

public class LocCounterTests
{
    [Fact]
    public void EmptyInputShouldReturnZero()
    {
        // Arrange
        var input = string.Empty;

        // Act
        var result = LocCounter.Count(input);

        // Assert
        result.Should().Be(0);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\r")]
    [InlineData("\n")]
    [InlineData("\r\n")]
    [InlineData("\f")] // form feed
    [InlineData("\v")] // vertical tab
    public void WhitespaceLinesShouldNotCountAsCode(string input)
    {
        // Whitespaces: https://learn.microsoft.com/en-us/cpp/c-language/white-space-characters?view=msvc-170

        // Act
        var results = LocCounter.Count(input);

        // Assert
        results.Should().Be(0);
    }

    [Fact]
    public void SingleLineCommentShouldReturnZero()
    {
        // Arrange
        var input = "// This is a comment";

        // Act
        var result = LocCounter.Count(input);

        // Assert
        result.Should().Be(0);
    }

    [Theory]
    [InlineData("var x = 1;")]
    [InlineData("  var x = 1;")]
    [InlineData("\tvar x = 1;")]
    [InlineData("{")]
    [InlineData("}")]
    [InlineData("var x = 1; // This is a comment")]
    public void CodeLinesShouldGetCounted(string input)
    {
        // Act
        var result = LocCounter.Count(input);

        // Assert
        result.Should().Be(1);
    }
}