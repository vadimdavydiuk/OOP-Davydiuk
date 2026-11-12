using System.Diagnostics;
using System.Globalization;
using SalesAnalytics;

Console.OutputEncoding = System.Text.Encoding.UTF8;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
Console.WriteLine("Самостійна №12: LINQ ETL та аналітика");
var random = new Random(5);
string[] categories = { "Електроніка", "Книги", "Одяг", "Спорт", "Дім" };
string[] cities = { "Рівне", "Київ", "Львів", "Луцьк", "Одеса" };
var products = Enumerable.Range(1, 100).Select(i => new Product(i, $"Товар {i}", categories[(i - 1) % 5], random.Next(100, 5001))).ToList();
var customers = Enumerable.Range(1, 100).Select(i => new Customer(i, $"Клієнт {i}", cities[(i - 1) % 5])).ToList();
var sales = Enumerable.Range(1, 100).Select(i => new Sale(i, random.Next(1, 101), random.Next(1, 11), new DateTime(2025, 1, 1).AddDays(random.Next(730)), random.Next(1, 101))).ToList();
Console.WriteLine($"Товарів: {products.Count}; клієнтів: {customers.Count}; продажів: {sales.Count}; seed=5");
var analytics = new Analytics(sales, products, customers);
var timingRows = new List<string> { "Pipeline,Milliseconds" };
Run("1. Топ-3 товари за категоріями", analytics.TopProductsByCategory);
Run("2. Дохід за містами", analytics.RevenueByCity);
Run("3. Продажі за роками та місяцями", analytics.MonthlySales);
Run("4. Топ-5 клієнтів за доходом у 2026 році", analytics.TopCustomers2026);
File.WriteAllLines(Path.Combine(AppContext.BaseDirectory, "measurements.csv"), timingRows);

void Run(string title, Func<List<string>> query)
{
    _ = query(); // Прогрів; вивід не включається до вимірювання.
    var watch = Stopwatch.StartNew();
    var lines = query();
    watch.Stop();
    Console.WriteLine($"\n{title}");
    foreach (var line in lines) Console.WriteLine(line);
    Console.WriteLine($"Час матеріалізації: {watch.Elapsed.TotalMilliseconds:F4} мс");
    timingRows.Add($"{title.Split('.')[0]},{watch.Elapsed.TotalMilliseconds:F6}");
}

namespace SalesAnalytics
{
    public record Sale(int Id, int ProductId, int Quantity, DateTime SaleDate, int CustomerId);
    public record Product(int Id, string Name, string Category, decimal UnitPrice);
    public record Customer(int Id, string Name, string City);
    public class Analytics(List<Sale> sales, List<Product> products, List<Customer> customers)
    {
        public List<string> TopProductsByCategory() => sales.GroupBy(s => s.ProductId)
            .Select(g => new { Id = g.Key, Quantity = g.Sum(s => s.Quantity) })
            .Join(products, s => s.Id, p => p.Id, (s, p) => new { p.Name, p.Category, p.Id, s.Quantity })
            .GroupBy(p => p.Category).OrderBy(g => g.Key)
            .SelectMany(g => g.OrderByDescending(p => p.Quantity).ThenBy(p => p.Id).Take(3)
                .Select(p => $"{g.Key}: {p.Name}, {p.Quantity} шт.")).ToList();
        public List<string> RevenueByCity() => sales
            .Join(products, s => s.ProductId, p => p.Id, (s, p) => new { s.CustomerId, Revenue = s.Quantity * p.UnitPrice })
            .Join(customers, s => s.CustomerId, c => c.Id, (s, c) => new { c.City, s.Revenue })
            .GroupBy(x => x.City).Select(g => new { City = g.Key, Revenue = g.Sum(x => x.Revenue) })
            .OrderByDescending(x => x.Revenue).ThenBy(x => x.City)
            .Select(x => $"{x.City}: {x.Revenue:F2} грн").ToList();
        public List<string> MonthlySales() => sales
            .Join(products, s => s.ProductId, p => p.Id, (s, p) => new { s.SaleDate, s.Quantity, Revenue = s.Quantity * p.UnitPrice })
            .GroupBy(x => new { x.SaleDate.Year, x.SaleDate.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => $"{g.Key.Year}-{g.Key.Month:D2}: {g.Sum(x => x.Quantity)} шт., {g.Sum(x => x.Revenue):F2} грн").ToList();
        public List<string> TopCustomers2026() => sales.Where(s => s.SaleDate.Year == 2026)
            .Join(products, s => s.ProductId, p => p.Id, (s, p) => new { s.CustomerId, Revenue = s.Quantity * p.UnitPrice })
            .GroupBy(x => x.CustomerId).Select(g => new { Id = g.Key, Revenue = g.Sum(x => x.Revenue) })
            .Join(customers, s => s.Id, c => c.Id, (s, c) => new { c.Id, c.Name, c.City, s.Revenue })
            .OrderByDescending(x => x.Revenue).ThenBy(x => x.Id).Take(5)
            .Select(x => $"{x.Name} ({x.City}): {x.Revenue:F2} грн").ToList();
    }
}
