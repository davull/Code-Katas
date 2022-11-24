namespace KataLocCorrect.Lib;

public static class LocCounterString
{
    private delegate void ContinuationAction(string text, ref int i);

    private const char Slash = '/';
    private const char Escape = '\\';
    private const char NewLine = '\n';
    private const char Star = '*';
    private const char Quote = '\"';

    public static int Count(string text)
    {
        var currentLineHasCode = false;
        var loc = 0;

        var newLineContinuation = new Action(() =>
        {
            if (currentLineHasCode)
                loc++;

            currentLineHasCode = false;
        });

        var codeContinuation = new ContinuationAction((string t, ref int i) =>
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
        string text,
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

    private static bool IsNewLine(string text, int i) => text[i] == NewLine;

    private static bool IsStartOfWhiteSpace(string text, int i) => char.IsWhiteSpace(text[i]);

    private static bool IsStartOfSingleLineComment(string text, int i)
    {
        if (text.Length < i + 1)
            return false;

        return text[i] == Slash && text[i + 1] == Slash;
    }

    private static bool IsStartOfMultiLineComment(string text, int i)
    {
        if (text.Length < i + 1)
            return false;

        return text[i] == Slash && text[i + 1] == Star;
    }

    private static bool IsStartOfAnyComment(string text, int i)
        => IsStartOfSingleLineComment(text, i) || IsStartOfMultiLineComment(text, i);

    private static bool IsStartOfString(string text, int i) => text[i] == Quote;

    private static bool IsEndOfString(string text, int i)
    {
        if (i == 0)
            return text[i] == Quote;

        // Handle Escape character
        return text[i - 1] != Escape && text[i] == Quote;
    }

    private static void ProcessWhiteSpace(string text, ref int i)
    {
        for (; i < text.Length; i++)
        {
            if (char.IsWhiteSpace(text[i]))
                continue;

            i--;
            break;
        }
    }

    private static void ProcessSingleLineComment(string text, ref int i)
    {
        for (; i < text.Length; i++)
        {
            if (text[i] != NewLine)
                continue;

            i--;
            break;
        }
    }

    private static void ProcessMultiLineComment(string text, ref int i)
    {
        for (; i < text.Length - 1; i++)
        {
            if (text[i] != Star || text[i + 1] != Slash)
                continue;

            i++;
            break;
        }
    }

    private static int ProcessCode(string text, ref int i)
    {
        var isStartOfCommentOrNewLine = (string text, int i) =>
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

    private static int ProcessString(string text, ref int i)
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