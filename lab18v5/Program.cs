using System;
using System.Collections.Generic;
using System.Text;

namespace lab18v5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var orderService = new OrderService();

            Console.WriteLine("=== 1. Демонстрація GetOrder (Використання власного винятку) ===\n");

            // Успішний пошук замовлення
            try
            {
                var existingOrder = orderService.GetOrder(101);
                Console.WriteLine($"[Успіх] Знайдено замовлення #{existingOrder.Id} для клієнта: {existingOrder.CustomerName}");
            }
            catch (OrderNotFoundException ex)
            {
                Console.WriteLine($"[Помилка] {ex.Message} (ID: {ex.OrderId})");
            }

            // Спроба отримати неіснуюче замовлення
            try
            {
                var missingOrder = orderService.GetOrder(999);
                Console.WriteLine($"[Успіх] Знайдено замовлення #{missingOrder.Id}");
            }
            catch (OrderNotFoundException ex)
            {
                Console.WriteLine($"[Перехоплено виняток] {ex.Message} | Проблемний OrderId: {ex.OrderId}");
            }

            Console.WriteLine("\n=== 2. Демонстрація PlaceOrder (Використання патерну Result<T>) ===\n");

            // Сценарій А: Товар є в наявності -> Result.Success
            var validOrder = new Order
            {
                Id = 102,
                CustomerName = "Олена Ковальчук",
                Items = new List<Product>
                {
                    new Product { Id = 1, Name = "Ноутбук", Quantity = 2 },
                    new Product { Id = 3, Name = "Клавіатура", Quantity = 1 }
                }
            };

            Result<bool> placeResult1 = orderService.PlaceOrder(validOrder);
            if (placeResult1.IsSuccess)
            {
                Console.WriteLine($"[Успіх] Замовлення #{validOrder.Id} успішно оформлено! Результат: {placeResult1.Value}");
            }
            else
            {
                Console.WriteLine($"[Відмова] Не вдалося оформити замовлення: {placeResult1.Error}");
            }

            // Сценарій Б: Товару немає в наявності -> Result.Failure
            var outOfStockOrder = new Order
            {
                Id = 103,
                CustomerName = "Максим Сидоренко",
                Items = new List<Product>
                {
                    new Product { Id = 2, Name = "Мишка", Quantity = 1 } // На складі 0 шт.
                }
            };

            Result<bool> placeResult2 = orderService.PlaceOrder(outOfStockOrder);
            if (placeResult2.IsSuccess)
            {
                Console.WriteLine($"[Успіх] Замовлення #{outOfStockOrder.Id} успішно оформлено!");
            }
            else
            {
                Console.WriteLine($"[Відмова] Не вдалося оформити замовлення #{outOfStockOrder.Id}: {placeResult2.Error}");
            }
        }
    }
}