using BenchmarkDotNet.Attributes;
using KataLocCorrect.Lib;

namespace KataLoc.Benchmarks;

[MemoryDiagnoser]
public class Benchmarks
{
    private string _text;

    [GlobalSetup]
    public void Setup()
    {
        var path = Path.Combine(Environment.CurrentDirectory,
            "../../../../../../../../KataLocCorrect/Lib/LocCounter.cs");

        _text = File.ReadAllText(path);
    }

    [Benchmark]
    public void Use_ref_string()
    {
        for (var i = 0; i < 100_000; i++)
            LocCounter.Count(_text);
    }

    [Benchmark]
    public void Use_string()
    {
        for (var i = 0; i < 100_000; i++)
            LocCounterString.Count(_text);
    }

    [Benchmark]
    public void Use_span()
    {
        for (var i = 0; i < 100_000; i++)
            LocCounterSpan.Count(_text);
    }
}