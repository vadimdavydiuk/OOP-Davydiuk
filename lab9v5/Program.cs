using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lab9DeliverySystem
{
    // Власний виняток для помилок доставки та валідації
    public class DeliveryException : Exception
    {
        public DeliveryException(string message) : base(message) { }
    }

    // 1. Абстрактний базовий клас
    public abstract class DeliveryService
    {
        public string OrderId { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public bool IsAvailableInStock { get; set; } = true;

        // Абстрактний метод доставки
        public abstract void Deliver(string address);

        // Спільна валідація наявності товару
        protected void ValidateStock()
        {
            if (!IsAvailableInStock)
            {
                throw new DeliveryException($"Товар '{ItemName}' (Замовлення #{OrderId}) відсутній на складі!");
            }
        }
    }

    // 2. Похідний клас 1: Кур'єрська доставка
    public class CourierDelivery : DeliveryService
    {
        public string CourierPhone { get; set; } = string.Empty;
        public string TimeSlot { get; set; } = "10:00-18:00";

        public override void Deliver(string address)
        {
            // Перевірка наявності товару
            ValidateStock();

            // Валідація адреси
            if (string.IsNullOrWhiteSpace(address) || address.Length < 10)
            {
                throw new DeliveryException($"[Кур'єр] Невалідна або занадто коротка адреса доставки: '{address}'");
            }

            // Валідація контактного телефону кур'єра
            if (string.IsNullOrWhiteSpace(CourierPhone) || !Regex.IsMatch(CourierPhone, @"^\+380\d{9}$"))
            {
                throw new DeliveryException($"[Кур'єр] Некоректний номер телефону кур'єра: '{CourierPhone}' (потрібен формат +380XXXXXXXXX)");
            }

            Console.WriteLine($"[КУР'ЄРСЬКА ДОСТАВКА] Замовлення #{OrderId} ('{ItemName}') доставляється кур'єром за адресою: \"{address}\". Час: {TimeSlot}. Тел. кур'єра: {CourierPhone}");
        }
    }

    // 3. Похідний клас 2: Поштова доставка (Нова Пошта / Укрпошта)
    public class PostDelivery : DeliveryService
    {
        public string PostalCode { get; set; } = string.Empty;
        public string BranchNumber { get; set; } = string.Empty;

        public override void Deliver(string address)
        {
            ValidateStock();

            if (string.IsNullOrWhiteSpace(address))
            {
                throw new DeliveryException("[Пошта] Назва міста/населеного пункту не може бути порожньою.");
            }

            // Перевірка поштового індексу (5 цифр)
            if (string.IsNullOrWhiteSpace(PostalCode) || !Regex.IsMatch(PostalCode, @"^\d{5}$"))
            {
                throw new DeliveryException($"[Пошта] Некоректний поштовий індекс: '{PostalCode}' (очікується 5 цифр)");
            }

            if (string.IsNullOrWhiteSpace(BranchNumber))
            {
                throw new DeliveryException("[Пошта] Номер поштового відділення не вказано.");
            }

            Console.WriteLine($"[ПОШТОВА ДОСТАВКА] Замовлення #{OrderId} ('{ItemName}') відправлено у м. {address}, Відділення №{BranchNumber} (Індекс: {PostalCode})");
        }
    }

    // 4. Похідний клас 3: Самовивіз з точки видачі
    public class PickupDelivery : DeliveryService
    {
        public string PickupPointId { get; set; } = string.Empty;

        public override void Deliver(string address)
        {
            ValidateStock();

            // Валідація ID точки видачі
            if (string.IsNullOrWhiteSpace(PickupPointId) || !PickupPointId.StartsWith("PUP-"))
            {
                throw new DeliveryException($"[Самовивіз] Некоректний ID точки видачі: '{PickupPointId}' (формат має починатися з 'PUP-')");
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                throw new DeliveryException("[Самовивіз] Адреса точки видачі не вказана.");
            }

            Console.WriteLine($"[САМОВИВІЗ] Замовлення #{OrderId} ('{ItemName}') готове до видачі в точці {PickupPointId} за адресою: \"{address}\"");
        }
    }

    // 5. Клас-сервіс для поліморфної обробки
    public class OrderDeliveryManager
    {
        public void ProcessAllDeliveries(List<DeliveryService> deliveries, string defaultAddress = "")
        {
            Console.WriteLine("=== РОЗПОЧАТО ОБРОБКУ ДОСТАВОК ===\n");

            int successCount = 0;
            int errorCount = 0;

            foreach (var delivery in deliveries)
            {
                try
                {
                    // Поліморфний виклик Deliver()
                    delivery.Deliver(defaultAddress);
                    successCount++;
                }
                catch (DeliveryException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[ПОМИЛКА ДОСТАВКИ] ({delivery.GetType().Name}): {ex.Message}");
                    Console.ResetColor();
                    errorCount++;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"[СИСТЕМНА ПОМИЛКА] ({delivery.GetType().Name}): {ex.Message}");
                    Console.ResetColor();
                    errorCount++;
                }
            }

            Console.WriteLine($"\n=== ПІДСУМОК: Успішно оброблено: {successCount} | З помилками: {errorCount} ===");
        }
    }

    // 6. Точка входу
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Створення списку об'єктів базового типу List<DeliveryService>
            List<DeliveryService> deliveryList = new List<DeliveryService>
            {
                // 1. Успішна кур'єрська доставка
                new CourierDelivery
                {
                    OrderId = "101",
                    ItemName = "Ноутбук ASUS",
                    IsAvailableInStock = true,
                    CourierPhone = "+380671112233",
                    TimeSlot = "14:00-16:00"
                },

                // 2. Успішна поштова доставка
                new PostDelivery
                {
                    OrderId = "102",
                    ItemName = "Смартфон Samsung",
                    IsAvailableInStock = true,
                    PostalCode = "33000",
                    BranchNumber = "12"
                },

                // 3. НЕВАЛІДНА доставка (Помилка: товару немає на складі)
                new CourierDelivery
                {
                    OrderId = "103",
                    ItemName = "Ігрова приставка PS5",
                    IsAvailableInStock = false, // Відсутній на складі!
                    CourierPhone = "+380509998877"
                },

                // 4. Успішний самовивіз
                new PickupDelivery
                {
                    OrderId = "104",
                    ItemName = "Навушники Sony",
                    IsAvailableInStock = true,
                    PickupPointId = "PUP-7788"
                },

                // 5. НЕВАЛІДНА поштова доставка (Помилка: неправильний поштовий індекс)
                new PostDelivery
                {
                    OrderId = "105",
                    ItemName = "Клавіатура",
                    IsAvailableInStock = true,
                    PostalCode = "3300", // Помилка: має бути 5 цифр
                    BranchNumber = "5"
                },

                // 6. НЕВАЛІДНИЙ самовивіз (Помилка: некоректний PickupPointId)
                new PickupDelivery
                {
                    OrderId = "106",
                    ItemName = "Мишка логітек",
                    IsAvailableInStock = true,
                    PickupPointId = "STORE-1" // Помилка: має починатися з 'PUP-'
                }
            };

            // Запуск сервісу обробки доставок
            OrderDeliveryManager manager = new OrderDeliveryManager();
            manager.ProcessAllDeliveries(deliveryList, "м. Рівне, вул. Соборна, 11");
        }
    }
}