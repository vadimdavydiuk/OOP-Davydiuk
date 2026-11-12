using Polly;
using Polly.Retry;
using Polly.CircuitBreaker;
using Polly.Timeout;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("Самостійна №14: Polly, три сценарії");
Console.WriteLine("\n1. Читання каталогу з БД: Retry після тимчасового розриву з'єднання.");
var retry = new ResiliencePipelineBuilder().AddRetry(new RetryStrategyOptions
{
    ShouldHandle = new PredicateBuilder().Handle<IOException>(),
    MaxRetryAttempts = 3, Delay = TimeSpan.FromMilliseconds(100),
    BackoffType = DelayBackoffType.Exponential, UseJitter = true,
    OnRetry = args => { Console.WriteLine($"Retry {args.AttemptNumber + 1}: {args.Outcome.Exception?.Message}; затримка {args.RetryDelay.TotalMilliseconds:F0} мс"); return default; }
}).Build();
int dbAttempts = 0;
string catalog = retry.Execute(() =>
{
    Console.WriteLine($"Звернення до БД #{++dbAttempts}");
    if (dbAttempts <= 2) throw new IOException("Тимчасовий розрив з'єднання.");
    return "Каталог: монітор, камера, навушники";
});
Console.WriteLine(catalog);

Console.WriteLine("\n2. Перевантажена черга: Circuit Breaker та перевірка відновлення.");
int queueCalls = 0;
bool queueHealthy = false;
var breaker = new ResiliencePipelineBuilder().AddCircuitBreaker(new CircuitBreakerStrategyOptions
{
    ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>(),
    FailureRatio = 1, MinimumThroughput = 2, SamplingDuration = TimeSpan.FromSeconds(5),
    BreakDuration = TimeSpan.FromMilliseconds(600),
    OnOpened = args => { Console.WriteLine("Circuit: OPEN"); return default; },
    OnHalfOpened = args => { Console.WriteLine("Circuit: HALF-OPEN"); return default; },
    OnClosed = args => { Console.WriteLine("Circuit: CLOSED"); return default; }
}).Build();
void SendToQueue()
{
    Console.WriteLine($"Реальне звернення до імітованої черги #{++queueCalls}");
    if (!queueHealthy) throw new HttpRequestException("Черга перевантажена.");
    Console.WriteLine("Повідомлення прийнято.");
}
for (int call = 1; call <= 3; call++)
{
    try { breaker.Execute(SendToQueue); }
    catch (BrokenCircuitException) { Console.WriteLine("Звернення заблоковано: callback не виконувався."); }
    catch (HttpRequestException ex) { Console.WriteLine(ex.Message); }
}
queueHealthy = true;
await Task.Delay(750);
breaker.Execute(SendToQueue);
Console.WriteLine($"Фактичних викликів: {queueCalls} (третій запит був заблокований).");

Console.WriteLine("\n3. Повільний експорт звіту: Timeout зі скасуванням.");
var timeout = new ResiliencePipelineBuilder().AddTimeout(new TimeoutStrategyOptions
{
    Timeout = TimeSpan.FromSeconds(1),
    OnTimeout = args => { Console.WriteLine("Timeout: перевищено 1 секунду."); return default; }
}).Build();
try
{
    await timeout.ExecuteAsync(async cancellationToken =>
    {
        Console.WriteLine("Експорт розпочато...");
        // Операція обов'язково враховує токен скасування.
        await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
        Console.WriteLine("Експорт завершено.");
    });
}
catch (TimeoutRejectedException) { Console.WriteLine("Експорт скасовано політикою Timeout."); }
Console.WriteLine("Усі зовнішні ресурси імітовані, мережевих запитів немає.");
