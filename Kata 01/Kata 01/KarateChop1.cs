namespace Kata_01;

/// <summary>
/// Simple iterative solution
/// </summary>
public class KarateChop1
{
    public int Chop(int[] arr, int search)
    {
        var lower = 0;
        var upper = arr.Length - 1;

        while (upper >= lower)
        {
            var mid = lower + (upper - lower) / 2;

            if (arr[mid] == search)
                return mid;

            if (arr[mid] > search)
                upper = mid - 1;
            else
                lower = mid + 1;
        }

        return -1;
    }
}