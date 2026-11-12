using System;

namespace IndependentWork2
{
    // Клас Product для представлення товару в інтернет-магазині
    public class Product
    {
        // Приватні поля
        private readonly int _id;
        private readonly string _name;
        private readonly decimal _price;
        private readonly string _category;
        private readonly int _stockCount;

        // Публічні властивості (read-only)
        public int Id => _id;
        public string Name => _name;
        public decimal Price => _price;
        public string Category => _category;
        public int StockCount => _stockCount;

        // Конструктор 1 (основний) - приймає всі 5 параметрів
        public Product(int id, string name, decimal price, string category, int stockCount)
        {
            _id = id;
            _name = name;
            _price = price;
            _category = category;
            _stockCount = stockCount;
        }

        // Конструктор 2 (для швидкого додавання товару)
        // Викликає основний конструктор через : this(...)
        public Product(int id, string name, decimal price)
            : this(id, name, price, "Uncategorized", 0)
        {
        }

        // Конструктор 3 (копіювання)
        // Викликає основний конструктор, передаючи дані з об'єкта-параметра
        public Product(Product other)
            : this(
                  other ?? throw new ArgumentNullException(nameof(other), "Об'єкт для копіювання не може бути null"),
                  other.Name,
                  other.Price,
                  other.Category,
                  other.StockCount
              )
        {
        }

        // Допоміжний приватний конструктор для безпечного виклику this(...) при перевірці на null
        private Product(Product other, string name, decimal price, string category, int stockCount)
            : this(other.Id, name, price, category, stockCount)
        {
        }

        // Перевизначення методу ToString()
        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}, Price: {Price:C}, Category: {Category}, Stock: {StockCount}";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Створення товарів");

            // 1. Створення об'єкта за допомогою основного конструктора
            Product product1 = new Product(101, "Laptop", 35000.00m, "Electronics", 15);
            Console.WriteLine($"Товар 1 (основний конструктор): {product1}");

            // 2. Створення об'єкта за допомогою скороченого конструктора
            Product product2 = new Product(102, "Mouse", 800.00m);
            Console.WriteLine($"Товар 2 (скорочений конструктор): {product2}");

            // 3. Створення об'єкта за допомогою конструктора копіювання
            Product product3 = new Product(product1);
            Console.WriteLine($"Товар 3 (конструктор копіювання): {product3}");
        }
    }
}