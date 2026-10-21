using System;

namespace lab12v5;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Демонстрація роботи InMemoryRepository<Customer> ===");

        IRepository<Customer> repository = new InMemoryRepository<Customer>();

        // 1. Додавання
        repository.Add(new Customer { Id = 1, FirstName = "Іван", LastName = "Петренко", Address = "м. Київ, вул. Хрещатик, 1" });
        repository.Add(new Customer { Id = 2, FirstName = "Олена", LastName = "Ковальчук", Address = "м. Рівне, вул. Соборна, 12" });

        // 2. Отримання всіх
        Console.WriteLine("\nУсі клієнти після додавання:");
        foreach (var c in repository.GetAll())
        {
            Console.WriteLine($"ID: {c.Id} | {c.FirstName} {c.LastName} | Адреса: {c.Address}");
        }

        // 3. Отримання за ID
        var customer = repository.GetById(1);
        Console.WriteLine($"\nЗнайдено клієнта за ID 1: {customer?.FirstName} {customer?.LastName}");

        // 4. Оновлення
        if (customer != null)
        {
            customer.Address = "м. Київ, вул. Перемоги, 10";
            repository.Update(customer);
            Console.WriteLine($"\nОновлена адреса клієнта ID 1: {repository.GetById(1)?.Address}");
        }

        // 5. Видалення
        repository.Delete(2);
        Console.WriteLine("\nУсі клієнти після видалення ID 2:");
        foreach (var c in repository.GetAll())
        {
            Console.WriteLine($"ID: {c.Id} | {c.FirstName} {c.LastName}");
        }
    }
}