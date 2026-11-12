using IndependentWork7;
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("Самостійна №8: FIFO-кеш");
var cache = new Cache<string, string>(2);
cache.Add("A", "Перший");
cache.Add("B", "Другий");
Console.WriteLine($"A = {cache.Get("A")}; Count = {cache.Count}");
cache.Add("C", "Третій");
Console.WriteLine($"Після додавання C: A = {cache.ContainsKey("A")}, B = {cache.ContainsKey("B")}, C = {cache.ContainsKey("C")}");
try { cache.Get("A"); }
catch (KeyNotFoundException ex) { Console.WriteLine(ex.Message); }

namespace IndependentWork7
{

public class Cache<TKey, TValue> where TKey : notnull
{
    private readonly int _capacity;
    private readonly Dictionary<TKey, TValue> _cache = new();
    private readonly Queue<TKey> _keysOrder = new();
    public int Count => _cache.Count;

    public Cache(int capacity)
    {
        if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));
        _capacity = capacity;
    }

    public void Add(TKey key, TValue value)
    {
        // Оновлення значення не змінює початковий порядок FIFO.
        if (_cache.ContainsKey(key))
        {
            _cache[key] = value;
            return;
        }
        if (_cache.Count == _capacity) _cache.Remove(_keysOrder.Dequeue());
        _cache.Add(key, value);
        _keysOrder.Enqueue(key);
    }

    public TValue Get(TKey key) => _cache.TryGetValue(key, out var value)
        ? value : throw new KeyNotFoundException($"Ключ '{key}' відсутній у кеші.");

    public bool ContainsKey(TKey key) => _cache.ContainsKey(key);
}

}
