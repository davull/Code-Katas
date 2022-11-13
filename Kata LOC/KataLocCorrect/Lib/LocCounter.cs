namespace KataLocCorrect.Lib;

public static class LocCounter
{
    private const char Slash = '/';
    private const char Escape = '\\';
    private const char NewLine = '\n';
    private const char Star = '*';
    private const char Quote = '\"';

    public static int Count(string text)
    {
        var currentLineHasCode = false;
        var loc = 0;

        for (var i = 0; i < text.Length; i++)
        {
            if (IsNewLine(ref text, i))
            {
                if (currentLineHasCode)
                    loc++;

                currentLineHasCode = false;
            }
            else if (IsStartOfWhiteSpace(ref text, i))
            {
                ProcessWhiteSpace(ref text, ref i);
            }
            else if (IsStartOfSingleLineComment(ref text, i))
            {
                ProcessSingleLineComment(ref text, ref i);
            }
            else if (IsStartOfMultiLineComment(ref text, i))
            {
                ProcessMultiLineComment(ref text, ref i);
            }
            else
            {
                loc += ProcessCode(ref text, ref i);
                currentLineHasCode = true;
            }
        }

        if (currentLineHasCode)
            loc++;

        return loc;
    }

    private static bool IsNewLine(ref string text, int i) => text[i] == NewLine;

    private static bool IsStartOfWhiteSpace(ref string text, int i) => char.IsWhiteSpace(text[i]);

    private static bool IsStartOfSingleLineComment(ref string text, int i)
    {
        if (text.Length < i + 1)
            return false;

        return text[i] == Slash && text[i + 1] == Slash;
    }

    private static bool IsStartOfMultiLineComment(ref string text, int i)
    {
        if (text.Length < i + 1)
            return false;

        return text[i] == Slash && text[i + 1] == Star;
    }

    private static bool IsStartOfAnyComment(ref string text, int i)
        => IsStartOfSingleLineComment(ref text, i) || IsStartOfMultiLineComment(ref text, i);

    private static bool IsStartOfString(ref string text, int i) => text[i] == Quote;

    private static bool IsEndOfString(ref string text, int i)
    {
        if ( i == 0 )
            return text[i] == Quote;

        // Handle Escape character
        return text[i - 1] != Escape && text[i] == Quote;
    }

    private static void ProcessWhiteSpace(ref string text, ref int i)
    {
        for (; i < text.Length; i++)
        {
            if (char.IsWhiteSpace(text[i]))
                continue;

            i--;
            break;
        }
    }

    private static void ProcessSingleLineComment(ref string text, ref int i)
    {
        for (; i < text.Length; i++)
        {
            if (text[i] != NewLine)
                continue;

            i--;
            break;
        }
    }

    private static void ProcessMultiLineComment(ref string text, ref int i)
    {
        for (; i < text.Length - 1; i++)
        {
            if (text[i] != Star || text[i + 1] != Slash)
                continue;

            i++;
            break;
        }
    }

    private static int ProcessCode(ref string text, ref int i)
    {
        var isStartOfCommentOfNewLine = (ref string text, int i) =>
            IsStartOfAnyComment(ref text, i) || IsNewLine(ref text, i);

        var newLinesInStrings = 0;

        for (; i < text.Length; i++)
        {
            if (IsStartOfString(ref text, i))
            {
                i++;
                newLinesInStrings += ProcessString(ref text, ref i);
                continue;
            }

            if (isStartOfCommentOfNewLine(ref text, i) == false)
                continue;

            i--;
            break;
        }

        return newLinesInStrings;
    }

    private static int ProcessString(ref string text, ref int i)
    {
        var newLinesInString = 0;

        for (; i < text.Length; i++)
        {
            if (IsNewLine(ref text, i))
                newLinesInString++;

            if(IsEndOfString(ref text, i))
                break;
        }

        return newLinesInString;
    }
}