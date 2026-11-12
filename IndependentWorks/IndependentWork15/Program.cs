using IndependentWork14;
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("Самостійна №15: exponential backoff з jitter ±20%");
var policy = new RetryPolicy(3, TimeSpan.FromMilliseconds(100), 0.2);
Func<Exception, bool> transient = ex => ex is IOException or HttpRequestException;
Console.WriteLine("\n1. Читання звіту: успіх після двох помилок.");
int attempts = 0;
string result = policy.Execute(() =>
{
    if (++attempts <= 2) throw new IOException("Звіт тимчасово зайнятий.");
    return "Звіт прочитано";
}, transient);
Console.WriteLine(result);
Console.WriteLine("\n2. Постійний збій сервера: остаточний виняток.");
try { policy.Execute<string>(() => throw new HttpRequestException("Сервер недоступний."), transient); }
catch (HttpRequestException ex) { Console.WriteLine("Оброблено після всіх спроб: " + ex.Message); }
Console.WriteLine("\n3. Невалідні аргументи: без повторів.");
try { policy.Execute<bool>(() => throw new ArgumentException("Невалідний шлях."), transient); }
catch (ArgumentException ex) { Console.WriteLine("Оброблено одразу: " + ex.Message); }

namespace IndependentWork14
{
public class RetryPolicy
{
    private readonly int _maxRetries;
    private readonly TimeSpan _initialDelay;
    private readonly double _jitterFactor;
    public RetryPolicy(int maxRetries, TimeSpan initialDelay, double jitterFactor = 0.2)
    {
        if (maxRetries < 0) throw new ArgumentOutOfRangeException(nameof(maxRetries));
        if (initialDelay < TimeSpan.Zero || initialDelay.TotalMilliseconds > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(initialDelay));
        if (!double.IsFinite(jitterFactor) || jitterFactor < 0 || jitterFactor > 1)
            throw new ArgumentOutOfRangeException(nameof(jitterFactor));
        _maxRetries = maxRetries; _initialDelay = initialDelay; _jitterFactor = jitterFactor;
    }
    public T Execute<T>(Func<T> operation, Func<Exception, bool>? shouldRetry = null)
    {
        ArgumentNullException.ThrowIfNull(operation);
        for (int retryAttempt = 0; ; retryAttempt++)
        {
            Console.WriteLine($"Спроба {retryAttempt + 1}; дозволено повторів: {_maxRetries}");
            try
            {
                T result = operation(); Console.WriteLine("Успіх."); return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
                if (retryAttempt >= _maxRetries || !(shouldRetry?.Invoke(ex) ?? (ex is IOException or HttpRequestException)))
                {
                    Console.WriteLine("Повтори припинено, виняток передається викликачу.");
                    throw;
                }
                // retryAttempt починається з нуля. Jitter дає множник [1-f, 1+f).
                double exponentialMs = _initialDelay == TimeSpan.Zero ? 0
                    : _initialDelay.TotalMilliseconds * Math.Pow(2, retryAttempt);
                double delayMs = exponentialMs * (1 + Random.Shared.NextDouble() * _jitterFactor * 2 - _jitterFactor);
                // Верхня межа потрібна для допустимого діапазону Thread.Sleep.
                delayMs = Math.Min(delayMs, int.MaxValue);
                Console.WriteLine($"Backoff з jitter: {delayMs:F2} мс.");
                Thread.Sleep(TimeSpan.FromMilliseconds(delayMs));
            }
        }
    }
}

}
