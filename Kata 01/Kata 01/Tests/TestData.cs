using System.Collections;

namespace Kata_01.Tests;

public class TestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        var data = new List<object[]>
        {
            // arr[], search, expected
            new object[] { Array.Empty<int>(), 3, -1 },
            new object[] { new[] { 1 }, 3, -1 },
            new object[] { new[] { 1 }, 1, 0 },
            // 
            new object[] { new[] { 1, 3, 5 }, 1, 0 },
            new object[] { new[] { 1, 3, 5 }, 3, 1 },
            new object[] { new[] { 1, 3, 5 }, 5, 2 },
            new object[] { new[] { 1, 3, 5 }, 0, -1 },
            new object[] { new[] { 1, 3, 5 }, 2, -1 },
            new object[] { new[] { 1, 3, 5 }, 4, -1 },
            new object[] { new[] { 1, 3, 5 }, 6, -1 },
            // 
            new object[] { new[] { 1, 3, 5, 7 }, 1, 0 },
            new object[] { new[] { 1, 3, 5, 7 }, 3, 1 },
            new object[] { new[] { 1, 3, 5, 7 }, 5, 2 },
            new object[] { new[] { 1, 3, 5, 7 }, 7, 3 },
            new object[] { new[] { 1, 3, 5, 7 }, 0, -1 },
            new object[] { new[] { 1, 3, 5, 7 }, 2, -1 },
            new object[] { new[] { 1, 3, 5, 7 }, 4, -1 },
            new object[] { new[] { 1, 3, 5, 7 }, 6, -1 },
            new object[] { new[] { 1, 3, 5, 7 }, 8, -1 },
        };

        return data.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}