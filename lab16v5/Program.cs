using System;
using System.Collections.Generic;
using System.Linq;

namespace lab16v5
{
    // Сутність: Пам'ятка
    public class Landmark
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString() => Name;
    }

    // Сутність: Місто
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Population { get; set; }
        public List<Landmark> Landmarks { get; set; } = new List<Landmark>();

        public override string ToString() => $"{Name} (Населення: {Population:N0}, Пам'яток: {Landmarks.Count})";
    }

    // Сутність: Країна
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<City> Cities { get; set; } = new List<City>();

        public override string ToString() => $"{Name} (Міст: {Cities.Count})";
    }

    // Клас для власних методів розширення (Custom Extension Methods)
    public static class ExtensionMethods
    {
        // 3. Custom Extension: Метод TotalPopulation() для IEnumerable<City>
        public static int TotalPopulation(this IEnumerable<City> cities)
        {
            return cities.Sum(c => c.Population);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== ЛАБОРАТОРНА РОБОТА №16 (Варіант 5: Країни, міста та пам'ятки) ===");
            Console.WriteLine("Тема: LINQ: SelectMany, пагінація й custom extensions.\n");

            // --- ТЕСТОВІ ДАНІ (Ієрархічні списки) ---
            var countries = new List<Country>
            {
                new Country
                {
                    Id = 1, Name = "Україна",
                    Cities = new List<City>
                    {
                        new City { Id = 101, Name = "Київ", Population = 2950000, Landmarks = new List<Landmark> { new Landmark { Id = 1, Name = "Софійський собор" }, new Landmark { Id = 2, Name = "Києво-Печерська лавра" } } },
                        new City { Id = 102, Name = "Львів", Population = 717000, Landmarks = new List<Landmark> { new Landmark { Id = 3, Name = "Площа Ринок" }, new Landmark { Id = 4, Name = "Високий Замок" } } },
                        new City { Id = 103, Name = "Рівне", Population = 245000, Landmarks = new List<Landmark> { new Landmark { Id = 5, Name = "Краєзнавчий музей" }, new Landmark { Id = 6, Name = "Лебедине озеро" } } }
                    }
                },
                new Country
                {
                    Id = 2, Name = "Франція",
                    Cities = new List<City>
                    {
                        new City { Id = 201, Name = "Париж", Population = 2140000, Landmarks = new List<Landmark> { new Landmark { Id = 7, Name = "Ейфелева вежа" }, new Landmark { Id = 8, Name = "Лувр" } } },
                        new City { Id = 202, Name = "Марсель", Population = 870000, Landmarks = new List<Landmark> { new Landmark { Id = 9, Name = "Старий порт" }, new Landmark { Id = 10, Name = "Базиліка Нотр-Дам-де-ла-Гард" } } }
                    }
                },
                new Country
                {
                    Id = 3, Name = "Італія",
                    Cities = new List<City>
                    {
                        new City { Id = 301, Name = "Рим", Population = 2750000, Landmarks = new List<Landmark> { new Landmark { Id = 11, Name = "Колізей" }, new Landmark { Id = 12, Name = "Ватикан" } } },
                        new City { Id = 302, Name = "Венеція", Population = 250000, Landmarks = new List<Landmark> { new Landmark { Id = 13, Name = "Площа Сан-Марко" }, new Landmark { Id = 14, Name = "Міст Ріальто" } } },
                        new City { Id = 303, Name = "Мілан", Population = 1350000, Landmarks = new List<Landmark> { new Landmark { Id = 15, Name = "Міланський собор" } } }
                    }
                }
            };

            // ==========================================
            // 1. SELECTMANY: Отримати всі пам'ятки всіх міст усіх країн в одному списку
            // ==========================================
            Console.WriteLine("--- 1. Демонстрація SelectMany (Усі пам'ятки в один список) ---");
            
            // Використовуємо SelectMany двічі для розгортання Cities, а потім Landmarks
            var allLandmarks = countries
                .SelectMany(c => c.Cities)
                .SelectMany(city => city.Landmarks)
                .ToList();

            Console.WriteLine($"Знайдено унікальних/загальних пам'яток: {allLandmarks.Count}");
            int counter = 1;
            foreach (var landmark in allLandmarks)
            {
                Console.WriteLine($"   {counter++}. {landmark.Name}");
            }
            Console.WriteLine();


            // ==========================================
            // 2. ПАГІНАЦІЯ (Skip / Take): Вивести всі міста по 4 міста на сторінку
            // ==========================================
            Console.WriteLine("--- 2. Демонстрація пагінації (Міста по 4 на сторінку) ---");
            
            // Збираємо абсолютно всі міста з усіх країн в один загальний список для зручності пагінації
            var allCities = countries.SelectMany(c => c.Cities).ToList();
            int pageSize = 4;
            int totalCities = allCities.Count;
            int totalPages = (int)Math.Ceiling(totalCities / (double)pageSize);

            for (int page = 1; page <= totalPages; page++)
            {
                Console.WriteLine($"\n[Сторінка {page} із {totalPages}]:");
                
                var citiesOnPage = allCities
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                foreach (var city in citiesOnPage)
                {
                    Console.WriteLine($"   * Місто: {city.Name}, Населення: {city.Population:N0}");
                }
            }
            Console.WriteLine();


            // ==========================================
            // 3. CUSTOM EXTENSION METHOD: TotalPopulation()
            // ==========================================
            Console.WriteLine("--- 3. Демонстрація власного методу розширення (TotalPopulation) ---");
            
            // Обчислюємо загальне населення всіх міст України за допомогою створеного extension методу
            var ukraineCities = countries.First(c => c.Name == "Україна").Cities;
            int ukraineTotalPop = ukraineCities.TotalPopulation();

            Console.WriteLine($"Країна: Україна");
            Console.WriteLine($"Перелік міст: {string.Join(", ", ukraineCities.Select(c => c.Name))}");
            Console.WriteLine($"Загальне населення (викликано через .TotalPopulation()): {ukraineTotalPop:N0} жителів");

            // Також можна застосувати до глобального списку міст
            int globalTotalPop = allCities.TotalPopulation();
            Console.WriteLine($"Сукупне населення всіх міст у базі: {globalTotalPop:N0} жителів");
        }
    }
}