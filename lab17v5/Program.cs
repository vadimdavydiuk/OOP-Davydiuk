using System.Text;
using System.Text.Json;

namespace lab17v5;

public static class RetryHelper
{
    // retryCount — кількість повторів після першого виклику.
    public static T ExecuteWithRetry<T>(Func<T> operation, int retryCount = 3,
        TimeSpan initialDelay = default, Func<Exception, bool>? shouldRetry = null)
    {
        ArgumentNullException.ThrowIfNull(operation);
        if (retryCount < 0) throw new ArgumentOutOfRangeException(nameof(retryCount));
        if (initialDelay < TimeSpan.Zero || initialDelay.TotalMilliseconds > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(initialDelay));

        double delayMs = initialDelay.TotalMilliseconds;
        for (int attempt = 0; ; attempt++)
        {
            Console.WriteLine($"Спроба {attempt + 1} (дозволено повторів: {retryCount})");
            try
            {
                T result = operation();
                Console.WriteLine("Операція успішна.");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка {ex.GetType().Name}: {ex.Message}");
                if (attempt >= retryCount || !(shouldRetry?.Invoke(ex)
                    ?? (ex is IOException or HttpRequestException)))
                {
                    Console.WriteLine("Повторення припинено.");
                    throw;
                }
                Console.WriteLine($"Затримка перед повтором: {delayMs:0} мс.");
                Thread.Sleep(TimeSpan.FromMilliseconds(delayMs));
                delayMs = Math.Min(delayMs * 2, int.MaxValue);
            }
        }
    }
}

public class FileProcessor
{
    private int attempts;
    public string? UpdatedConfigJson { get; private set; }

    public void UpdateConfig(string path, string key, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(value);
        if (++attempts <= 2)
            throw new IOException("Файл конфігурації тимчасово зайнятий.");

        // Імітація оновлення: зовнішні файли не потрібні.
        UpdatedConfigJson = JsonSerializer.Serialize(new Dictionary<string, string> { [key] = value });
        Console.WriteLine($"Конфігурацію {path} оновлено: {key} = {value}");
    }
}

public class NetworkClient
{
    private int attempts;
    public bool SendConfigUpdate(string url, string configJson)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)
            || (uri.Scheme != "https" && uri.Scheme != "http"))
            throw new ArgumentException("Потрібна коректна HTTP/HTTPS адреса.", nameof(url));
        using var document = JsonDocument.Parse(configJson);
        if (++attempts <= 3)
            throw new HttpRequestException("Сервер конфігурації тимчасово недоступний.");
        Console.WriteLine($"Оновлення надіслано до {url}: {configJson}");
        return true;
    }
}

internal class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("=== Лабораторна №17. Варіант 5: Оновлення конфігурації ===");
        Func<Exception, bool> shouldRetry = ex => ex is IOException or HttpRequestException;
        var processor = new FileProcessor();
        var client = new NetworkClient();
        try
        {
            Console.WriteLine("\n1. Оновлення файлу: дві помилки, потім успіх.");
            // Обгортка перетворює void-операцію на Func<bool>.
            RetryHelper.ExecuteWithRetry(() =>
            {
                processor.UpdateConfig("settings.json", "Theme", "Dark");
                return true;
            }, initialDelay: TimeSpan.FromMilliseconds(100), shouldRetry: shouldRetry);

            Console.WriteLine("\n2. Надсилання: три помилки, потім успіх.");
            bool sent = RetryHelper.ExecuteWithRetry(
                () => client.SendConfigUpdate("https://example.com/config", processor.UpdatedConfigJson!),
                initialDelay: TimeSpan.FromMilliseconds(100), shouldRetry: shouldRetry);
            Console.WriteLine($"Результат надсилання: {sent}");

            Console.WriteLine("\n3. Невалідна адреса: shouldRetry забороняє повтор.");
            try
            {
                RetryHelper.ExecuteWithRetry(() => client.SendConfigUpdate("invalid", "{}"),
                    shouldRetry: shouldRetry);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Оброблено без повтору: {ex.Message}");
            }

            Console.WriteLine("\n4. Вичерпання ліміту: дозволено лише один повтор.");
            var unavailableClient = new NetworkClient();
            try
            {
                RetryHelper.ExecuteWithRetry(
                    () => unavailableClient.SendConfigUpdate("https://example.com/config", "{}"),
                    retryCount: 1, initialDelay: TimeSpan.FromMilliseconds(100), shouldRetry: shouldRetry);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Ліміт вичерпано: {ex.Message}");
            }
        }
        catch (Exception ex) when (ex is IOException or HttpRequestException)
        {
            Console.WriteLine($"Операцію не завершено: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("\nДемонстрацію завершено (finally).");
        }
    }
}
