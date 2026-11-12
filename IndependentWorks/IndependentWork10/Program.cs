using CollectionUtilities;
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("Самостійна №10: Делегати та лямбда-вирази");
var numbers = new List<int> { -3, 2, 7, 4, 10 };
var words = new List<string> { "кеш", "колекція", "делегат", "код" };
var products = new List<Product> { new("Монітор", 6500), new("Навушники", 1200), new("Камера", 3100) };
Console.WriteLine("Квадрати: " + string.Join(", ", CollectionUtils.Map(numbers, n => n * n)));
Console.WriteLine("Парні: " + string.Join(", ", CollectionUtils.Filter(numbers, n => n % 2 == 0)));
Console.WriteLine("Максимальне число: " + CollectionUtils.MaxBy(numbers, n => n));
CollectionUtils.ForEach(numbers, n => Console.Write($"[{n}] ")); Console.WriteLine();
Console.WriteLine("Довжини слів: " + string.Join(", ", CollectionUtils.Map(words, w => w.Length)));
Console.WriteLine("Довгі слова: " + string.Join(", ", CollectionUtils.Filter(words, w => w.Length > 4)));
Console.WriteLine("Найдовше: " + CollectionUtils.MaxBy(words, w => w.Length));
CollectionUtils.ForEach(words, w => Console.Write(w.ToUpperInvariant() + " ")); Console.WriteLine();
Console.WriteLine("Назви: " + string.Join(", ", CollectionUtils.Map(products, p => p.Name)));
Console.WriteLine("Найдорожчий: " + CollectionUtils.MaxBy(products, p => p.Price));
CollectionUtils.ForEach(CollectionUtils.Filter(products, p => p.Price > 2000), p => Console.WriteLine($"Дорожчий за 2000: {p.Name}"));
CollectionUtils.ForEach(products, p => Console.WriteLine($"{p.Name}: {p.Price} грн"));
try { CollectionUtils.MaxBy(Array.Empty<int>(), n => n); }
catch (InvalidOperationException ex) { Console.WriteLine("Порожня колекція: " + ex.Message); }

namespace CollectionUtilities
{
public static class CollectionUtils
{
    public static IEnumerable<TResult> Map<TSource, TResult>(IEnumerable<TSource> collection, Func<TSource, TResult> mapper)
    {
        ArgumentNullException.ThrowIfNull(collection); ArgumentNullException.ThrowIfNull(mapper);
        var result = new List<TResult>();
        foreach (var item in collection) result.Add(mapper(item));
        return result;
    }
    public static T MaxBy<T, TKey>(IEnumerable<T> collection, Func<T, TKey> keySelector) where TKey : IComparable<TKey>
    {
        ArgumentNullException.ThrowIfNull(collection); ArgumentNullException.ThrowIfNull(keySelector);
        using var iterator = collection.GetEnumerator();
        if (!iterator.MoveNext()) throw new InvalidOperationException("Колекція порожня.");
        T maxItem = iterator.Current;
        TKey maxKey = keySelector(maxItem);
        while (iterator.MoveNext())
        {
            TKey key = keySelector(iterator.Current);
            if (Comparer<TKey>.Default.Compare(key, maxKey) > 0) { maxItem = iterator.Current; maxKey = key; }
        }
        return maxItem;
    }
    public static IEnumerable<T> Filter<T>(IEnumerable<T> collection, Predicate<T> predicate)
    {
        ArgumentNullException.ThrowIfNull(collection); ArgumentNullException.ThrowIfNull(predicate);
        var result = new List<T>();
        foreach (var item in collection) if (predicate(item)) result.Add(item);
        return result;
    }
    public static void ForEach<T>(IEnumerable<T> collection, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(collection); ArgumentNullException.ThrowIfNull(action);
        foreach (var item in collection) action(item);
    }
}
public record Product(string Name, decimal Price);

}
