using System.Collections.Generic;
using System.Linq;

namespace lab18v5
{
    public class OrderService
    {
        private readonly List<Order> _orders = new List<Order>();
        private readonly Dictionary<int, int> _warehouseStock = new Dictionary<int, int>
        {
            { 1, 10 }, // Ноутбук: 10 шт. в наявності
            { 2, 0 },  // Мишка: 0 шт. (немає в наявності)
            { 3, 5 }   // Клавіатура: 5 шт. в наявності
        };

        public OrderService()
        {
            // Попередньо додаємо одне існуюче замовлення для тестування GetOrder
            _orders.Add(new Order
            {
                Id = 101,
                CustomerName = "Іван Петренко",
                Items = new List<Product>
                {
                    new Product { Id = 1, Name = "Ноутбук", Quantity = 1 }
                }
            });
        }

        // Метод викидає власний виняток, якщо замовлення не знайдено
        public Order GetOrder(int orderId)
        {
            var order = _orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                throw new OrderNotFoundException(orderId);
            }

            return order;
        }

        // Метод повертає Result<bool> для очікуваних бізнес-помилок (немає товару)
        public Result<bool> PlaceOrder(Order order)
        {
            if (order == null || order.Items == null || order.Items.Count == 0)
            {
                return Result<bool>.Failure("Замовлення не може бути порожнім.");
            }

            // Перевіряємо наявність кожного товару на складі
            foreach (var item in order.Items)
            {
                if (!_warehouseStock.TryGetValue(item.Id, out int availableQty) || availableQty < item.Quantity)
                {
                    return Result<bool>.Failure($"Товар '{item.Name}' (ID: {item.Id}) відсутній на складі в потрібній кількості.");
                }
            }

            // Списуємо товари зі складу та зберігаємо замовлення
            foreach (var item in order.Items)
            {
                _warehouseStock[item.Id] -= item.Quantity;
            }

            _orders.Add(order);
            return Result<bool>.Success(true);
        }
    }
}