using System.Text;

namespace lab18v5;

public class OrderNotFoundException : Exception
{
    public int OrderId { get; }
    public OrderNotFoundException() : base("Замовлення не знайдено.") { }
    public OrderNotFoundException(string message) : base(message) { }
    public OrderNotFoundException(string message, Exception innerException)
        : base(message, innerException) { }
    public OrderNotFoundException(int orderId) : base($"Замовлення №{orderId} не знайдено.")
    {
        OrderId = orderId;
    }
}

public class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public string Error { get; }

    private Result(T value, bool isSuccess, string error)
    {
        Value = value;
        IsSuccess = isSuccess;
        Error = error;
    }
    public static Result<T> Success(T value) => new(value, true, string.Empty);
    // У випадку невдачі Value не використовується: спочатку перевіряємо IsSuccess.
    public static Result<T> Failure(string error) => new(default!, false, error);
    public static Result<T> Failure(Exception ex)
    {
        ArgumentNullException.ThrowIfNull(ex);
        return Failure(ex.Message);
    }
}

public record OrderItem(int ProductId, int Quantity);
public record Order(int Id, string Customer, IReadOnlyList<OrderItem> Items);

public class OrderService
{
    private readonly Dictionary<int, int> stock = new() { [1] = 5, [2] = 0, [3] = 10 };
    private readonly Dictionary<int, Order> orders = new();

    public Order GetOrder(int orderId)
    {
        if (!orders.TryGetValue(orderId, out var order))
            throw new OrderNotFoundException(orderId);
        return order;
    }

    public Result<bool> PlaceOrder(Order order)
    {
        if (order is null || order.Id <= 0 || string.IsNullOrWhiteSpace(order.Customer)
            || order.Items is null || order.Items.Count == 0)
            return Result<bool>.Failure("Некоректні дані або порожнє замовлення.");
        if (orders.ContainsKey(order.Id))
            return Result<bool>.Failure($"Замовлення №{order.Id} вже існує.");
        if (order.Items.Any(item => item is null || item.ProductId <= 0 || item.Quantity <= 0))
            return Result<bool>.Failure("Кількість та ідентифікатор товару мають бути додатними.");

        // Об'єднуємо повторені товари, щоб не перевищити залишок кількома рядками.
        var quantities = order.Items.GroupBy(item => item.ProductId)
            .ToDictionary(group => group.Key, group => group.Sum(item => (long)item.Quantity));
        foreach (var (productId, quantity) in quantities)
        {
            if (!stock.TryGetValue(productId, out int available) || available < quantity)
                return Result<bool>.Failure($"Товар №{productId}: потрібно {quantity}, у наявності {available}.");
        }

        // Змінюємо залишки лише після перевірки всіх товарів. Зберігаємо копію списку.
        var savedOrder = order with { Items = Array.AsReadOnly(order.Items.ToArray()) };
        foreach (var (productId, quantity) in quantities)
            stock[productId] -= (int)quantity;
        orders.Add(order.Id, savedOrder);
        return Result<bool>.Success(true);
    }
}

internal class Program
{
    static void ShowResult(Result<bool> result)
    {
        if (result.IsSuccess)
            Console.WriteLine($"Замовлення оформлено. Value = {result.Value}");
        else
            Console.WriteLine($"Замовлення не оформлено: {result.Error}");
    }

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("=== Лабораторна №18. Варіант 5: Управління замовленнями ===");
        var service = new OrderService();
        Console.WriteLine("\n1. Успішне оформлення (Result).");
        ShowResult(service.PlaceOrder(new Order(1, "Давидюк", new[] { new OrderItem(1, 2) })));

        Console.WriteLine("\n2. Товар відсутній (Result).");
        ShowResult(service.PlaceOrder(new Order(2, "Давидюк", new[] { new OrderItem(2, 1) })));

        Console.WriteLine("\n3. Недостатня сумарна кількість повтореного товару (Result).");
        ShowResult(service.PlaceOrder(new Order(3, "Давидюк",
            new[] { new OrderItem(1, 2), new OrderItem(1, 2) })));

        Console.WriteLine("\n4. Повторний номер замовлення (Result).");
        ShowResult(service.PlaceOrder(new Order(1, "Давидюк", new[] { new OrderItem(3, 1) })));

        Console.WriteLine("\n5. Пошук наявного та відсутнього замовлень (try-catch).");
        try
        {
            var order = service.GetOrder(1);
            Console.WriteLine($"Знайдено замовлення №{order.Id}, покупець: {order.Customer}.");
            service.GetOrder(999);
        }
        catch (OrderNotFoundException ex)
        {
            Console.WriteLine($"Власний виняток: {ex.Message} OrderId = {ex.OrderId}");
            var result = Result<bool>.Failure(ex);
            Console.WriteLine($"Failure(Exception): IsSuccess = {result.IsSuccess}, Error = {result.Error}");
        }
    }
}
