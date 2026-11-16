using System;
using System.Threading;

namespace lab17v5
{
    public static class RetryHelper
    {
        public static T ExecuteWithRetry<T>(
            Func<T> operation,
            int retryCount = 3,
            TimeSpan initialDelay = default,
            Func<Exception, bool>? shouldRetry = null)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            if (initialDelay == default)
                initialDelay = TimeSpan.FromMilliseconds(500);

            int attempt = 0;

            while (true)
            {
                try
                {
                    attempt++;
                    Console.WriteLine($"[RetryHelper] Спроба #{attempt}...");
                    T result = operation();
                    Console.WriteLine($"[RetryHelper] Операція успішно виконана на спробі #{attempt}.");
                    return result;
                }
                catch (Exception ex)
                {
                    int currentRetry = attempt - 1;
                    bool canRetry = currentRetry < retryCount && (shouldRetry == null || shouldRetry(ex));

                    if (!canRetry)
                    {
                        Console.WriteLine($"[RetryHelper] Зупинка після спроби #{attempt}. Причина ({ex.GetType().Name}): {ex.Message}");
                        throw;
                    }

                    // Експоненційна затримка: initialDelay * 2^(currentRetry)
                    double multiplier = Math.Pow(2, currentRetry);
                    TimeSpan delay = TimeSpan.FromMilliseconds(initialDelay.TotalMilliseconds * multiplier);

                    Console.WriteLine($"[RetryHelper] Помилка ({ex.GetType().Name}): {ex.Message}. Повторна спроба {currentRetry + 1}/{retryCount} через {delay.TotalMilliseconds} мс...");
                    Thread.Sleep(delay);
                }
            }
        }

        // Перевантаження для void-методів (наприклад, UpdateConfig)
        public static void ExecuteWithRetry(
            Action operation,
            int retryCount = 3,
            TimeSpan initialDelay = default,
            Func<Exception, bool>? shouldRetry = null)
        {
            ExecuteWithRetry<bool>(() =>
            {
                operation();
                return true;
            }, retryCount, initialDelay, shouldRetry);
        }
    }
}