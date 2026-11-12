using System.Diagnostics;
using System.Globalization;

Console.OutputEncoding = System.Text.Encoding.UTF8;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
Console.WriteLine($"Самостійна №13: LINQ та PLINQ; логічних процесорів: {Environment.ProcessorCount}");
var rows = new List<string> { "Count,Operation,LINQms,PLINQms,Speedup,Result" };
foreach (int size in new[] { 1_000_000, 5_000_000, 10_000_000 })
{
    var random = new Random(5);
    var data = Enumerable.Range(0, size).Select(_ => random.Next(2, 100000)).ToList();
    Compare("Квадрат", n => (long)n * n);
    Compare("Просте число", n => IsPrime(n) ? 1L : 0L);
    void Compare(string name, Func<int, long> operation)
    {
        long Linear() => data.Where(n => n > 0).Select(operation).Sum();
        long Parallel() => data.AsParallel().Where(n => n > 0).Select(operation).Sum();
        if (Linear() != Parallel()) throw new InvalidOperationException("Результати різні.");
        var linear = new List<double>(); var parallel = new List<double>();
        for (int run = 0; run < 3; run++)
        {
            // Чергуємо порядок, щоб не завжди давати другому підходу теплий кеш.
            if (run % 2 == 0) { linear.Add(Time(Linear)); parallel.Add(Time(Parallel)); }
            else { parallel.Add(Time(Parallel)); linear.Add(Time(Linear)); }
        }
        linear.Sort(); parallel.Sort();
        Console.WriteLine($"N={size}, {name}: LINQ={linear[1]:F3} мс; PLINQ={parallel[1]:F3} мс; прискорення={linear[1] / parallel[1]:F2}; результат={Linear()}");
        rows.Add($"{size},{name},{linear[1]:F6},{parallel[1]:F6},{linear[1] / parallel[1]:F4},{Linear()}");
    }
}
File.WriteAllLines(Path.Combine(AppContext.BaseDirectory, "measurements.csv"), rows);
Console.WriteLine("\nГонка даних: очікується 100000 інкрементів у кожному запуску.");
for (int run = 1; run <= 5; run++)
{
    int unsafeCount = 0;
    ParallelEnumerable.Range(0, 100000).WithDegreeOfParallelism(Math.Max(2, Math.Min(4, Environment.ProcessorCount)))
        .ForAll(_ => { int snapshot = unsafeCount; Thread.SpinWait(20); unsafeCount = snapshot + 1; });
    Console.WriteLine($"Без синхронізації, запуск {run}: {unsafeCount}");
}
int safeCount = 0;
ParallelEnumerable.Range(0, 100000).ForAll(_ => Interlocked.Increment(ref safeCount));
Console.WriteLine($"Interlocked: {safeCount}");
if (safeCount != 100000) throw new InvalidOperationException("Помилка синхронізації.");
Console.WriteLine("Без спільного стану, PLINQ Sum: " + ParallelEnumerable.Range(0, 100000).Select(_ => 1).Sum());

static bool IsPrime(int n)
{
    if (n < 2) return false;
    if (n % 2 == 0) return n == 2;
    for (int divisor = 3; divisor * divisor <= n; divisor += 2) if (n % divisor == 0) return false;
    return true;
}
static double Time(Func<long> operation)
{
    var watch = Stopwatch.StartNew(); _ = operation(); watch.Stop(); return watch.Elapsed.TotalMilliseconds;
}
