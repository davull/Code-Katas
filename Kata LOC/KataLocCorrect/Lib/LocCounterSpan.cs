namespace KataLocCorrect.Lib;

public static class LocCounterSpan
{
    private delegate void ContinuationAction(ReadOnlySpan<char> text, ref int i);

    private const char Slash = '/';
    private const char Escape = '\\';
    private const char NewLine = '\n';
    private const char Star = '*';
    private const char Quote = '\"';

    public static int Count(string text)
    {
        var span = text.AsSpan();
        return Count(span);
    }

    public static int Count(ReadOnlySpan<char> text)
    {
        var currentLineHasCode = false;
        var loc = 0;

        var newLineContinuation = new Action(() =>
        {
            if (currentLineHasCode)
                loc++;

            currentLineHasCode = false;
        });

        var codeContinuation = new ContinuationAction((ReadOnlySpan<char> t, ref int i) =>
        {
            loc += ProcessCode(t, ref i);
            currentLineHasCode = true;
        });

        for (var i = 0; i < text.Length; i++)
            ProcessChar(
                text: text,
                i: ref i,
                newLineContinuation: newLineContinuation,
                startOfWhiteSpaceContinuation: ProcessWhiteSpace,
                startOfSingleLineCommentContinuation: ProcessSingleLineComment,
                startOfMultiLineCommentContinuation: ProcessMultiLineComment,
                codeContinuation: codeContinuation);

        if (currentLineHasCode)
            loc++;

        return loc;
    }

    private static void ProcessChar(
        ReadOnlySpan<char> text,
        ref int i,
        Action newLineContinuation,
        ContinuationAction startOfWhiteSpaceContinuation,
        ContinuationAction startOfSingleLineCommentContinuation,
        ContinuationAction startOfMultiLineCommentContinuation,
        ContinuationAction codeContinuation)
    {
        if (IsNewLine(text, i))
            newLineContinuation();
        else if (IsStartOfWhiteSpace(text, i))
            startOfWhiteSpaceContinuation(text, ref i);
        else if (IsStartOfSingleLineComment(text, i))
            startOfSingleLineCommentContinuation(text, ref i);
        else if (IsStartOfMultiLineComment(text, i))
            startOfMultiLineCommentContinuation(text, ref i);
        else
            codeContinuation(text, ref i);
    }

    private static bool IsNewLine(ReadOnlySpan<char> text, int i) => text[i] == NewLine;

    private static bool IsStartOfWhiteSpace(ReadOnlySpan<char> text, int i) => char.IsWhiteSpace(text[i]);

    private static bool IsStartOfSingleLineComment(ReadOnlySpan<char> text, int i)
    {
        if (text.Length < i + 1)
            return false;

        return text[i] == Slash && text[i + 1] == Slash;
    }

    private static bool IsStartOfMultiLineComment(ReadOnlySpan<char> text, int i)
    {
        if (text.Length < i + 1)
            return false;

        return text[i] == Slash && text[i + 1] == Star;
    }

    private static bool IsStartOfAnyComment(ReadOnlySpan<char> text, int i)
        => IsStartOfSingleLineComment(text, i) || IsStartOfMultiLineComment(text, i);

    private static bool IsStartOfString(ReadOnlySpan<char> text, int i) => text[i] == Quote;

    private static bool IsEndOfString(ReadOnlySpan<char> text, int i)
    {
        if (i == 0)
            return text[i] == Quote;

        // Handle Escape character
        return text[i - 1] != Escape && text[i] == Quote;
    }

    private static void ProcessWhiteSpace(ReadOnlySpan<char> text, ref int i)
    {
        for (; i < text.Length; i++)
        {
            if (char.IsWhiteSpace(text[i]))
                continue;

            i--;
            break;
        }
    }

    private static void ProcessSingleLineComment(ReadOnlySpan<char> text, ref int i)
    {
        for (; i < text.Length; i++)
        {
            if (text[i] != NewLine)
                continue;

            i--;
            break;
        }
    }

    private static void ProcessMultiLineComment(ReadOnlySpan<char> text, ref int i)
    {
        for (; i < text.Length - 1; i++)
        {
            if (text[i] != Star || text[i + 1] != Slash)
                continue;

            i++;
            break;
        }
    }

    private static int ProcessCode(ReadOnlySpan<char> text, ref int i)
    {
        var isStartOfCommentOrNewLine = (ReadOnlySpan<char> text, int i) =>
            IsStartOfAnyComment(text, i) || IsNewLine(text, i);

        var newLinesInStrings = 0;

        for (; i < text.Length; i++)
        {
            if (IsStartOfString(text, i))
            {
                i++;
                newLinesInStrings += ProcessString(text, ref i);
                continue;
            }

            if (isStartOfCommentOrNewLine(text, i) == false)
                continue;

            i--;
            break;
        }

        return newLinesInStrings;
    }

    private static int ProcessString(ReadOnlySpan<char> text, ref int i)
    {
        var newLinesInString = 0;

        for (; i < text.Length; i++)
        {
            if (IsNewLine(text, i))
                newLinesInString++;

            if (IsEndOfString(text, i))
                break;
        }

        return newLinesInString;
    }
}