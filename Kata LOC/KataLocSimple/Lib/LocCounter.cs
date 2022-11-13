namespace KataLocSimple.Lib;

public static class LocCounter
{
    private const string SingleLineCommentToken = "//";

    public static int Count(string text)
    {
        var lines = GetLines(ref text);
        var linesWithCode = GetLinesWithCode(lines);

        return linesWithCode.Count();
    }

    private static IEnumerable<string> GetLines(ref string text) =>
        text.Split(
            separator: new[] { "\n", "\r\n", Environment.NewLine },
            options: StringSplitOptions.TrimEntries);

    private static IEnumerable<string> GetLinesWithCode(IEnumerable<string> lines) =>
        lines
            .Where(LineIsNotWhitespace)
            .Where(LineIsNotSingleLineComment);

    private static bool LineIsNotWhitespace(string line) =>
        !string.IsNullOrWhiteSpace(line);

    private static bool LineIsNotSingleLineComment(string line) =>
        !line.StartsWith(SingleLineCommentToken);
}