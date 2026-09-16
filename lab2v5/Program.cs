using System;

namespace lab2v5
{
    public class City
    {
        private string _name;
        private string _country;
        private long _population;

        public string Name
        {
            get => _name;
            set => _name = string.IsNullOrWhiteSpace(value) ? "Unknown" : value;
        }

        public string Country
        {
            get => _country;
            set => _country = string.IsNullOrWhiteSpace(value) ? "Unknown" : value;
        }

        public long Population
        {
            get => _population;
            set
            {
                if (value < 0)
                {
                    Console.WriteLine("Помилка: Населення не може бути від'ємним!");
                    _population = 0;
                }
                else
                {
                    _population = value;
                }
            }
        }

        public City() : this("Unknown", "Unknown", 0)
        {
        }

        public City(string name, string country, long population)
        {
            _name = string.IsNullOrWhiteSpace(name) ? "Unknown" : name;
            _country = string.IsNullOrWhiteSpace(country) ? "Unknown" : country;
            Population = population;
        }

        ~City()
        {
            Console.WriteLine($"[Деструктор]: Об'єкт міста \"{_name}\" знищено.");
        }

        public string GetCityInfo()
        {
            return $"Місто: {Name}, Країна: {Country}, Населення: {Population} осіб.";
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Creating objects ---");
            
            City city1 = new City();
            City city2 = new City("Рівне", "Україна", 245000);
            City city3 = new City("Тестове місто", "Держава", -500);

            Console.WriteLine("\n--- Displaying Info ---");
            Console.WriteLine(city1.GetCityInfo());
            Console.WriteLine(city2.GetCityInfo());
            Console.WriteLine(city3.GetCityInfo());

            Console.WriteLine("\n--- Objects created ---");
            Console.WriteLine("End of Main, preparing for GC...\n");

            city1 = null!;
            city2 = null!;
            city3 = null!;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}