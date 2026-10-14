using System;
using System.Collections.Generic;
using System.Threading;

namespace lab11v5
{
    // Абстрактний базовий клас, що реалізує патерн Template Method
    public abstract class GameLevel
    {
        public string LevelName { get; set; }
        public int DifficultyRating { get; set; }

        protected GameLevel(string levelName, int difficultyRating)
        {
            LevelName = levelName;
            DifficultyRating = difficultyRating;
        }

        // Шаблонний метод (Template Method) — визначає скелет алгоритму
        public void Play()
        {
            Console.WriteLine($"\n==========================================");
            Console.WriteLine($" Запуск рівня: {LevelName} (Складність: {DifficultyRating}/10)");
            Console.WriteLine($"==========================================");

            LoadLevel();
            SpawnEnemies();
            StartTimer();
            CheckWinCondition();
            ShowResults();

            Console.WriteLine($"[Система] Рівень {LevelName} завершено.\n");
        }

        // 1. Абстрактний крок: завантаження ресурсів рівня (обов'язковий для реалізації)
        protected abstract void LoadLevel();

        // 2. Абстрактний крок: спавн ворогів (обов'язковий для реалізації)
        protected abstract void SpawnEnemies();

        // 3. Віртуальний крок: запуск таймера (має базову реалізацію, але може бути перевизначений)
        protected virtual void StartTimer()
        {
            Console.WriteLine("[Таймер] Загальний відлік часу: 5 хвилин...");
        }

        // 4. Абстрактний крок: перевірка умов перемоги (обов'язковий для реалізації)
        protected abstract void CheckWinCondition();

        // 5. Віртуальний крок: показ результатів (має базову реалізацію)
        protected virtual void ShowResults()
        {
            Console.WriteLine("[Результати] Зараховано стандартну кількість балів та очок досвіду.");
        }
    }

    // 1. Похідний клас: Легкий рівень
    public class EasyLevel : GameLevel
    {
        public int ExtraLives { get; set; }

        public EasyLevel() : base("Початкова Долина", 3)
        {
            ExtraLives = 5;
        }

        protected override void LoadLevel()
        {
            Console.WriteLine($"[EasyLevel] Завантаження текстур низької роздільної здатності та навчальних матеріалів.");
        }

        protected override void SpawnEnemies()
        {
            Console.WriteLine($"[EasyLevel] З'явилося 3 слабких вороги-слимаки. Живіть спокійно, у вас {ExtraLives} додаткових життів.");
        }

        protected override void StartTimer()
        {
            // Перевищуємо віртуальний метод — на легкому рівні даємо більше часу
            Console.WriteLine("[Таймер] Режим навчання: таймер відключено (необмежений час).");
        }

        protected override void CheckWinCondition()
        {
            Console.WriteLine("[Умова перемоги] Зберіть хоча б 1 кристалик і дійдіть до виходу.");
        }
    }

    // 2. Похідний клас: Середній рівень
    public class MediumLevel : GameLevel
    {
        public bool CheckpointsEnabled { get; set; }

        public MediumLevel() : base("Загублений Ліс", 6)
        {
            CheckpointsEnabled = true;
        }

        protected override void LoadLevel()
        {
            Console.WriteLine("[MediumLevel] Завантаження стандартного середовища, динамічного освітлення та погодних ефектів.");
        }

        protected override void SpawnEnemies()
        {
            Console.WriteLine("[MediumLevel] Спавн зграї вовків та лісових гоблінів по всьому периметру.");
        }

        // Використовує базовий StartTimer() без перевизначення

        protected override void CheckWinCondition()
        {
            Console.WriteLine("[Умова перемоги] Переможіть боса локації та закрийте портал (активні чекпоинти: " + (CheckpointsEnabled ? "Так" : "Ні") + ").");
        }

        protected override void ShowResults()
        {
            base.ShowResults();
            Console.WriteLine("[Бонус] Нараховано подвійний множник досвіду (Medium Multiplier x1.5).");
        }
    }

    // 3. Похідний клас: Складний рівень
    public class HardLevel : GameLevel
    {
        public bool Permadeath { get; set; }

        public HardLevel() : base("Цитадель Темряви", 10)
        {
            Permadeath = true;
        }

        protected override void LoadLevel()
        {
            Console.WriteLine("[HardLevel] Завантаження важких текстур високої деталізації та високорівневої геометрії лабіринту.");
        }

        protected override void SpawnEnemies()
        {
            Console.WriteLine("[HardLevel] Увага! Спавн елітних драконів та нежиті. Пермадеф (Permadeath): " + (Permadeath ? "Увімкнено" : "Вимкнено"));
        }

        protected override void StartTimer()
        {
            Console.WriteLine("[Таймер] Хардкор-режим: жорсткий ліміт на проходження — рівно 2 хвилини!");
        }

        protected override void CheckWinCondition()
        {
            Console.WriteLine("[Умова перемоги] Пройдіть лабіринт без смертей та знищите головного артефакта сімох світів.");
        }

        protected override void ShowResults()
        {
            Console.WriteLine("[Результати] Легендарне проходження! Отримано унікальне досягнення та максимальну кількість очок слави.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Створення списку об'єктів базового типу GameLevel
            List<GameLevel> levels = new List<GameLevel>
            {
                new EasyLevel(),
                new MediumLevel(),
                new HardLevel()
            };

            // Виклик шаблонного методу для кожного рівня у поліморфний спосіб
            foreach (var level in levels)
            {
                level.Play();
            }
        }
    }
}