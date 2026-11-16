using System;
using System.IO;

namespace lab17v5
{
    public class FileProcessor
    {
        private int _attemptCounter = 0;

        public void UpdateConfig(string path, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Шлях до файлу конфігурації не може бути порожнім.", nameof(path));

            _attemptCounter++;

            // Імітуємо IOException перші 2 рази
            if (_attemptCounter <= 2)
            {
                throw new IOException($"Файл '{path}' заблоковано іншим процесом (спроба {_attemptCounter}/2).");
            }

            Console.WriteLine($"[FileProcessor] Конфігурацію у файлі '{path}' оновлено: {key} = {value}");
        }

        public void ResetCounter() => _attemptCounter = 0;
    }
}