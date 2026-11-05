using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace lab15v5
{
    // Сутність: Клієнт
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }

        public override string ToString() => $"{Name} (Місто: {City})";
    }

    // Сутність: Замовлення
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalAmount { get; set; }

        public override string ToString() => $"Замовлення #{Id} від {OrderDate.ToShortDateString()} на суму {TotalAmount} грн (Клієнт ID: {CustomerId})";
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== ЛАБОРАТОРНА РОБОТА №15 (Варіант 5: Управління замовленнями) ===");
            Console.WriteLine("Тема: LINQ в дії: фільтри, join, агрегації. Продуктивність.\n");

            // --- ТЕСТОВІ ДАНІ (10-20 елементів) ---
            var customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "Олександр Давидюк", City = "Kyiv" },
                new Customer { Id = 2, Name = "Марія Ковальчук", City = "Lviv" },
                new Customer { Id = 3, Name = "Іван Петренко", City = "Kyiv" },
                new Customer { Id = 4, Name = "Софія Мельник", City = "Rivne" },
                new Customer { Id = 5, Name = "Андрій Шевченко", City = "Kyiv" }
            };

            var orders = new List<Order>
            {
                new Order { Id = 101, OrderDate = new DateTime(2026, 5, 10), CustomerId = 1, TotalAmount = 1200m },
                new Order { Id = 102, OrderDate = new DateTime(2026, 5, 12), CustomerId = 2, TotalAmount = 850m },
                new Order { Id = 103, OrderDate = new DateTime(2026, 6, 1), CustomerId = 1, TotalAmount = 2500m },
                new Order { Id = 104, OrderDate = new DateTime(2026, 6, 15), CustomerId = 3, TotalAmount = 950m },
                new Order { Id = 105, OrderDate = new DateTime(2026, 7, 3), CustomerId = 3, TotalAmount = 1500m },
                new Order { Id = 106, OrderDate = new DateTime(2026, 7, 20), CustomerId = 4, TotalAmount = 450m },
                new Order { Id = 107, OrderDate = new DateTime(2026, 8, 5), CustomerId = 5, TotalAmount = 3100m },
                new Order { Id = 108, OrderDate = new DateTime(2026, 8, 10), CustomerId = 2, TotalAmount = 1100m },
                new Order { Id = 109, OrderDate = new DateTime(2026, 9, 1), CustomerId = 1, TotalAmount = 600m },
                new Order { Id = 110, OrderDate = new DateTime(2026, 9, 14), CustomerId = 5, TotalAmount = 1800m }
            };

            Stopwatch stopwatch = new Stopwatch();

            // ==========================================
            // ЗАПИТ 1: Фільтрація (Where)
            // Умова: Знайти всі замовлення від клієнтів з міста “Kyiv” на суму > 1000.
            // ==========================================
            Console.WriteLine("--- ЗАПИТ 1: Фільтрація (Клієнти з Києва, сума > 1000) ---");
            
            // А) Синтаксис методів (Method Syntax)
            stopwatch.Restart();
            var query1Methods = orders
                .Where(o => o.TotalAmount > 1000 && customers.First(c => c.Id == o.CustomerId).City == "Kyiv")
                .ToList();
            stopwatch.Stop();
            long time1M = stopwatch.ElapsedTicks;

            // Б) Синтаксис запитів (Query Syntax)
            stopwatch.Restart();
            var query1Query = (from o in orders
                               join c in customers on o.CustomerId equals c.Id
                               where c.City == "Kyiv" && o.TotalAmount > 1000
                               select o).ToList();
            stopwatch.Stop();
            long time1Q = stopwatch.ElapsedTicks;

            Console.WriteLine($"Результати (Query Syntax):");
            foreach (var item in query1Query)
            {
                Console.WriteLine($"   * {item}");
            }
            Console.WriteLine($"Час виконання (Method Syntax): {time1M} ticks | (Query Syntax): {time1Q} ticks\n");


            // ==========================================
            // ЗАПИТ 2: Об’єднання (Join)
            // Умова: Об’єднати замовлення з їхніми клієнтами, вивести дату замовлення та ім’я клієнта.
            // ==========================================
            Console.WriteLine("--- ЗАПИТ 2: Об’єднання даних (Join) ---");

            // А) Синтаксис методів
            stopwatch.Restart();
            var query2Methods = orders
                .Join(customers,
                      o => o.CustomerId,
                      c => c.Id,
                      (o, c) => new { o.OrderDate, CustomerName = c.Name })
                .ToList();
            stopwatch.Stop();
            long time2M = stopwatch.ElapsedTicks;

            // Б) Синтаксис запитів
            stopwatch.Restart();
            var query2Query = (from o in orders
                               join c in customers on o.CustomerId equals c.Id
                               select new { o.OrderDate, CustomerName = c.Name }).ToList();
            stopwatch.Stop();
            long time2Q = stopwatch.ElapsedTicks;

            Console.WriteLine("Результати об'єднання (перші 5 елементів):");
            foreach (var item in query2Query.Take(5))
            {
                Console.WriteLine($"   * Дата: {item.OrderDate.ToShortDateString()} | Клієнт: {item.CustomerName}");
            }
            Console.WriteLine($"Час виконання (Method Syntax): {time2M} ticks | (Query Syntax): {time2Q} ticks\n");


            // ==========================================
            // ЗАПИТ 3: Агрегація (GroupBy + Sum)
            // Умова: Підрахувати загальну суму замовлень для кожного клієнта.
            // ==========================================
            Console.WriteLine("--- ЗАПИТ 3: Агрегація даних (Сума замовлень по клієнтах) ---");

            // А) Синтаксис методів
            stopwatch.Restart();
            var query3Methods = orders
                .GroupBy(o => o.CustomerId)
                .Select(g => new { 
                    CustomerId = g.Key, 
                    CustomerName = customers.First(c => c.Id == g.Key).Name,
                    TotalSpent = g.Sum(o => o.TotalAmount) 
                })
                .ToList();
            stopwatch.Stop();
            long time3M = stopwatch.ElapsedTicks;

            // Б) Синтаксис запитів
            stopwatch.Restart();
            var query3Query = (from o in orders
                               join c in customers on o.CustomerId equals c.Id
                               group o by new { c.Id, c.Name } into g
                               select new { CustomerName = g.Key.Name, TotalSpent = g.Sum(o => o.TotalAmount) }).ToList();
            stopwatch.Stop();
            long time3Q = stopwatch.ElapsedTicks;

            Console.WriteLine("Результати агрегації:");
            foreach (var item in query3Query)
            {
                Console.WriteLine($"   * Клієнт: {item.CustomerName} — Загальна сума: {item.TotalSpent} грн");
            }
            Console.WriteLine($"Час виконання (Method Syntax): {time3M} ticks | (Query Syntax): {time3Q} ticks\n");


            // ==========================================
            // ЗАПИТ 4: Пошук екстремуму (Max / Aggregate)
            // Умова: Знайти замовлення з максимальною сумою.
            // ==========================================
            Console.WriteLine("--- ЗАПИТ 4: Пошук максимального значення (Max) ---");

            // А) Синтаксис методів
            stopwatch.Restart();
            var maxOrderMethod = orders.OrderByDescending(o => o.TotalAmount).FirstOrDefault();
            stopwatch.Stop();
            long time4M = stopwatch.ElapsedTicks;

            // Б) Синтаксис запитів
            stopwatch.Restart();
            var maxOrderQuery = (from o in orders
                                 orderby o.TotalAmount descending
                                 select o).FirstOrDefault();
            stopwatch.Stop();
            long time4Q = stopwatch.ElapsedTicks;

            Console.WriteLine($"   * Замовлення з макс. сумою: {maxOrderQuery}");
            Console.WriteLine($"Час виконання (Method Syntax): {time4M} ticks | (Query Syntax): {time4Q} ticks\n");

            // --- ВИСНОВКИ ПРО ПРОДУКТИВНІСТЬ ---
            Console.WriteLine("=== ВИСНОВКИ ЩОДО ПРОДУКТИВНОСТІ ===" );
            Console.WriteLine("1. Обидва синтаксиси (Method та Query) генерують практично ідентичний умовно оптимізований IL-код.");
            Console.WriteLine("2. Операції з'єднання (`join`) та групування (`group by`) є складнішими за просту фільтрацію (`where`), оскільки вимагають побудови хеш-таблиць у пам'яті для зіставлення ключів.");
            Console.WriteLine("3. Використання класу `Stopwatch` показує, що мікросекундні різниці між синтаксисами мінімальні, проте синтаксис методів часто виявляється трохи гнучкішим для складних ланцюжків операцій.");
        }
    }
}