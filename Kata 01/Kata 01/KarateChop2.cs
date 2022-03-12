namespace Kata_01;

/// <summary>
/// Simple recursive solution
/// </summary>
public class KarateChop2
{
    public int Chop(int[] arr, int search) => 
        Chop(arr: arr, search: search, start: 0, end: arr.Length - 1);

    private int Chop(IReadOnlyList<int> arr, int search, int start, int end)
    {
        if (end < start)
            return -1;
        
        var index = start + (end - start) / 2;

        if (arr[index] == search)
            return index;

        return arr[index] > search 
            ? Chop(arr: arr, search: search, start: start, end: index - 1) 
            : Chop(arr: arr, search: search, start: index + 1, end: end);
    }
}