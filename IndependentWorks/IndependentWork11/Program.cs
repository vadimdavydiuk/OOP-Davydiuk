using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Globalization;
using InvertedIndexDemo;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("Самостійна №11: Інвертований індекс");
var samples = new List<Document>
{
    new(1, "Introduction to C#", "C# is a modern object-oriented programming language developed by Microsoft."),
    new(2, "Java Basics", "Java is a popular programming language known for its portability and performance."),
    new(3, "Python Overview", "Python is a high-level programming language with simple syntax."),
    new(4, "C# Advanced Features", "C# supports LINQ, async/await, and many modern programming paradigms."),
    new(5, "Comparing Languages", "C#, Java, and Python are among the most popular programming languages today.")
};
var index = new InvertedIndex();
foreach (var doc in samples) index.AddDocument(doc);
Console.WriteLine($"Документів: {index.DocumentCount}; унікальних слів: {index.UniqueWords}");
Console.WriteLine("Без стоп-слів: " + string.Join(", ", TextProcessor.RemoveStopWords(TextProcessor.Tokenize(samples[0].Content))));
Console.WriteLine("AND programming language: " + string.Join(", ", index.SearchMultipleWords("programming", "language")));
Console.WriteLine("AND c# async: " + string.Join(", ", index.SearchMultipleWords("c#", "async")));
Console.WriteLine("OR python async: " + string.Join(", ", index.SearchAnyWord("python", "async")));
Console.WriteLine("Вимірювання: медіана 5 серій по 20 запитів, після прогріву; побудова окремо.");
var rows = new List<string> { "Documents,BuildMs,LinearMs,IndexMs,Found,BreakEvenQueries" };
foreach (int count in new[] { 10, 100, 1000, 10000 })
{
    var documents = Enumerable.Range(1, count).Select(i => samples[(i - 1) % 5] with { Id = i }).ToList();
    var measured = new InvertedIndex();
    var build = Stopwatch.StartNew();
    foreach (var doc in documents) measured.AddDocument(doc);
    build.Stop();
    List<Document> Linear() => documents.Where(d => TextProcessor.Tokenize(d.Content).Contains("c#")).ToList();
    List<Document> Indexed() => measured.SearchSingleWord("C#");
    if (!Linear().Select(d => d.Id).Order().SequenceEqual(Indexed().Select(d => d.Id).Order()))
        throw new InvalidOperationException("Результати пошуку не збігаються.");
    double linearMs = Measure(Linear), indexMs = Measure(Indexed);
    double breakEven = linearMs > indexMs ? Math.Ceiling(build.Elapsed.TotalMilliseconds / (linearMs - indexMs)) : double.PositiveInfinity;
    Console.WriteLine($"N={count}: побудова={build.Elapsed.TotalMilliseconds:F3} мс; linear={linearMs:F4} мс; index={indexMs:F4} мс; знайдено={Indexed().Count}; окупність≈{breakEven} запитів");
    rows.Add(FormattableString.Invariant($"{count},{build.Elapsed.TotalMilliseconds:F6},{linearMs:F6},{indexMs:F6},{Indexed().Count},{breakEven}"));
}
File.WriteAllLines(Path.Combine(AppContext.BaseDirectory, "measurements.csv"), rows);
if (!args.Contains("--demo"))
{
    while (true)
    {
        Console.Write("Запит AND (exit — вихід): ");
        string? query = Console.ReadLine();
        if (query is null || query.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;
        var results = index.SearchMultipleWords(query);
        foreach (var doc in results) Console.WriteLine($"{doc}\n  {doc.Content[..Math.Min(100, doc.Content.Length)]}");
        Console.WriteLine($"Знайдено: {results.Count}");
    }
}

static double Measure(Func<List<Document>> action)
{
    action();
    var times = new List<double>();
    for (int run = 0; run < 5; run++)
    {
        var watch = Stopwatch.StartNew();
        for (int i = 0; i < 20; i++) _ = action();
        watch.Stop(); times.Add(watch.Elapsed.TotalMilliseconds / 20);
    }
    times.Sort(); return times[2];
}

namespace InvertedIndexDemo
{
public record Document(int Id, string Title, string Content)
{
    public override string ToString() => $"[{Id}] {Title}";
}
public static class TextProcessor
{
    private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
        { "a", "an", "the", "is", "are", "was", "were", "in", "on", "at", "to", "for", "of", "and", "or" };
    // Зберігаємо C# та C++; дефіс і / розділяють слова.
    public static IEnumerable<string> Tokenize(string text) =>
        Regex.Matches(text, @"[\p{L}\p{N}]+(?:#|\+\+)?").Select(m => m.Value.ToLowerInvariant());
    public static IEnumerable<string> RemoveStopWords(IEnumerable<string> words) => words.Where(w => !StopWords.Contains(w));
}
public class InvertedIndex
{
    private readonly Dictionary<string, HashSet<int>> _index = new();
    private readonly Dictionary<int, Document> _documents = new();
    public int DocumentCount => _documents.Count;
    public int UniqueWords => _index.Count;
    public void AddDocument(Document doc)
    {
        ArgumentNullException.ThrowIfNull(doc);
        // Новий документ із тим самим ID замінює попередній, прибираючи старі слова.
        if (_documents.TryGetValue(doc.Id, out var old))
            foreach (string word in TextProcessor.Tokenize(old.Content).Distinct())
            {
                _index[word].Remove(doc.Id);
                if (_index[word].Count == 0) _index.Remove(word);
            }
        _documents[doc.Id] = doc;
        foreach (string word in TextProcessor.Tokenize(doc.Content))
        {
            if (!_index.TryGetValue(word, out var ids)) _index[word] = ids = new();
            ids.Add(doc.Id);
        }
    }
    private HashSet<int> FindIds(string word) => _index.TryGetValue(word, out var ids) ? new(ids) : new();
    private List<Document> Resolve(HashSet<int> ids) => ids.Order().Select(id => _documents[id]).ToList();
    public List<Document> SearchSingleWord(string word)
    {
        var tokens = TextProcessor.Tokenize(word).ToArray();
        return tokens.Length == 1 ? Resolve(FindIds(tokens[0])) : new();
    }
    public List<Document> SearchMultipleWords(params string[] words)
    {
        var tokens = words.SelectMany(TextProcessor.Tokenize).Distinct().ToArray();
        if (tokens.Length == 0) return new();
        var result = FindIds(tokens[0]);
        foreach (var word in tokens.Skip(1)) result.IntersectWith(FindIds(word));
        return Resolve(result);
    }
    public List<Document> SearchAnyWord(params string[] words)
    {
        var result = new HashSet<int>();
        foreach (var word in words.SelectMany(TextProcessor.Tokenize)) result.UnionWith(FindIds(word));
        return Resolve(result);
    }
}

}
