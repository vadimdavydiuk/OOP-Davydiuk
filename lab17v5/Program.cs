using System;
using System.IO;
using System.Net.Http;
using System.Text;

namespace lab17v5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            var fileProcessor = new FileProcessor();
            var networkClient = new NetworkClient();

            // Делегат визначає, які винятки вважаються тимчасовими
            Func<Exception, bool> shouldRetryPolicy = ex =>
                ex is IOException || ex is HttpRequestException;

            Console.WriteLine("=== 1. Сценарій FileProcessor.UpdateConfig (2 невдачі -> успіх) ===\n");
            try
            {
                RetryHelper.ExecuteWithRetry(
                    operation: () => fileProcessor.UpdateConfig("appsettings.json", "MaxConnections", "100"),
                    retryCount: 3,
                    initialDelay: TimeSpan.FromMilliseconds(300),
                    shouldRetry: shouldRetryPolicy
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Main] Фінальна помилка: {ex.Message}");
            }

            Console.WriteLine("\n=== 2. Сценарій NetworkClient.SendConfigUpdate (3 невдачі -> успіх) ===\n");
            try
            {
                bool isSent = RetryHelper.ExecuteWithRetry(
                    operation: () => networkClient.SendConfigUpdate("https://api.config.local/update", "{\"MaxConnections\":100}"),
                    retryCount: 4,
                    initialDelay: TimeSpan.FromMilliseconds(200),
                    shouldRetry: shouldRetryPolicy
                );
                Console.WriteLine($"[Main] Статус відправки: {isSent}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Main] Фінальна помилка: {ex.Message}");
            }

            Console.WriteLine("\n=== 3. Демонстрація фільтрації shouldRetry (критична помилка без повторів) ===\n");
            try
            {
                // ArgumentException не входить у shouldRetryPolicy, тому повторних спроб не буде
                RetryHelper.ExecuteWithRetry(
                    operation: () => fileProcessor.UpdateConfig("", "Theme", "Dark"),
                    retryCount: 3,
                    initialDelay: TimeSpan.FromMilliseconds(300),
                    shouldRetry: shouldRetryPolicy
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Main] Операцію перервано без повторних спроб: {ex.GetType().Name} - {ex.Message}");
            }
        }
    }
}