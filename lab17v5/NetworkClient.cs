using System;
using System.Net.Http;

namespace lab17v5
{
    public class NetworkClient
    {
        private int _attemptCounter = 0;

        public bool SendConfigUpdate(string url, string configJson)
        {
            if (url.Contains("forbidden"))
                throw new UnauthorizedAccessException("Доступ до вузла заборонено (403 Forbidden).");

            _attemptCounter++;

            // Імітуємо HttpRequestException перші 3 рази
            if (_attemptCounter <= 3)
            {
                throw new HttpRequestException($"Тимчасова помилка мережі при підключенні до {url} (спроба {_attemptCounter}/3).");
            }

            Console.WriteLine($"[NetworkClient] Конфігурацію '{configJson}' успішно надіслано на {url}");
            return true;
        }

        public void ResetCounter() => _attemptCounter = 0;
    }
}