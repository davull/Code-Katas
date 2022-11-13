using FluentAssertions;
using KataLocCorrect.Lib;
using Xunit;

namespace KataLocCorrect.Tests;

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

    [Theory]
    [InlineData("var x = 1;")]
    [InlineData("  var x = 1;")]
    [InlineData("\tvar x = 1;")]
    [InlineData("{")]
    [InlineData("}")]
    [InlineData("var x = 1; // This is a comment")]
    [InlineData("var x = 1 / 2;")]
    [InlineData("var x = \"\";")]
    public void CodeLinesShouldGetCounted(string input)
    {
        // Act
        var result = LocCounter.Count(input);

        // Assert
        result.Should().Be(1);
    }

    [Theory]
    [InlineData("var x = 1;\r\n var y = 2;", 2)]
    [InlineData("var x = 1; \r\n var y = 2;", 2)]
    [InlineData("var x = 1; \r\n\r\n var y = 2;", 2)]
    [InlineData("\r\nvar x = 1;\r\nvar y = 2;\r\n", 2)]
    public void MultipleCodeLinesShouldGetCounted(string input, int expected)
    {
        // Act
        var result = LocCounter.Count(input);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("// This is a comment")]
    [InlineData("// This is a comment \r\n")]
    [InlineData("  // This is a comment  ")]
    [InlineData("  // This is a comment  \r\n")]
    [InlineData("//")]
    [InlineData("//\r\n")]
    public void SingleLineCommentShouldReturnZero(string input)
    {
        // Act
        var result = LocCounter.Count(input);

        // Assert
        result.Should().Be(0);
    }

    [Theory]
    [InlineData("// line 1\r\n// line 2", 0)]
    [InlineData("\r\n // line 1 \r\n // line 2 \r\n", 0)]
    [InlineData("// comment 1\r\nvar i = 0;", 1)]
    [InlineData("// comment 1\r\nvar i = 0;\r\n // comment 2", 1)]
    [InlineData("// comment 1\r\nvar i = 0;\r\n // comment 2\r\n var j = 10;", 2)]
    [InlineData("// comment 1\r\nvar i = 0;\r\n // comment 2\r\n var j = 10; \r\n // comment 3", 2)]
    public void MultipleSingleLineCommentsShouldNotGetCounted(string input, int expected)
    {
        // Act
        var result = LocCounter.Count(input);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("/* this is my comment */")]
    [InlineData("\r\n/* this is my comment */")]
    [InlineData("/* this is my comment */\r\n")]
    [InlineData("/* this \r\n is my \r\n comment */  ")]
    [InlineData("  /* this \n is my \n comment */")]
    public void MultiLineCommentShouldReturnZero(string input)
    {
        // Act
        var result = LocCounter.Count(input);

        // Assert
        result.Should().Be(0);
    }

    [Theory]
    [InlineData("var i = 0; /* comment */")]
    [InlineData("/* comment */ var i = 0;")]
    [InlineData("var i = 0; /* comment */ i = 1;")]
    public void MultiLineCommentWithCodeShouldGetCounted(string input)
    {
        // Act
        var result = LocCounter.Count(input);

        // Assert
        result.Should().Be(1);
    }

    [Theory]
    [InlineData("var s = \"line1 \r\n line2\";", 2)]
    [InlineData("var s = \"line1 \r\n // comment \r\n line2\";", 3)]
    [InlineData("var s = \"line1 \r\n /* comment */ \r\n line2\";", 3)]
    [InlineData("var s = $\"line1 \r\n line2\";", 2)]
    [InlineData("var s = @\"line1 \r\n line2\";", 2)]
    [InlineData("var s = $@\"line1 \r\n line2\";", 2)]
    [InlineData("var s = @$\" {1+2} \r\n line2\";", 2)]
    public void MultiLineStringsShouldGetCounted(string input, int expected)
    {
        // Act
        var result = LocCounter.Count(input);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void IgnoreEscapedQuotesInStrings()
    {
        // Arrange
        var input = @"

var str = "" hello \"" 
  this is still the string
  // a comment inside my string"";

";
        var expected = 3;

        // Act
        var result = LocCounter.Count(input);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void MultipleMultiLineStringsShouldGetCounted()
    {
        // Arrange
        var input = @"

var s1 = ""line 1
           line 2""; var s2 = ""line 3
line4"";
";
        var expected = 3;

        // Act
        var result = LocCounter.Count(input);

        // Assert
        result.Should().Be(expected);
    }
}