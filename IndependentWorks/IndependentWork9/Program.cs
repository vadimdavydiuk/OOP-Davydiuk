using System.Text.RegularExpressions;
using IndependentWork9;
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("Самостійна №9: Dictionary і HashSet");
var cache = new FileMetadataCache();
cache.Add("report.txt", new("report.txt", 2048, new DateTime(2026, 1, 1), "demo-hash"));
Console.WriteLine($"Кеш: {cache.Get("REPORT.TXT")}; Contains = {cache.Contains("report.txt")}");
Console.WriteLine($"Видалено: {cache.Remove("report.txt")}; Contains = {cache.Contains("report.txt")}");
var tags = new TagManager();
tags.AddTag("1", "C#"); tags.AddTag("1", "c#"); tags.AddTag("1", "LINQ");
tags.AddTag("2", "C#"); tags.AddTag("2", "OOP");
Console.WriteLine("Усі теги: " + string.Join(", ", tags.GetAllUniqueTags().Order()));
Console.WriteLine("Спільні: " + string.Join(", ", tags.FindCommonTags("1", "2")));
Console.WriteLine("Лише у першій: " + string.Join(", ", tags.FindExclusiveTags("1", "2")));
tags.RemoveTag("2", "OOP");
Console.WriteLine("Після видалення OOP: " + string.Join(", ", tags.GetAllUniqueTags().Order()));
var index = new SearchIndex();
index.AddDocument(1, "C# LINQ collections"); index.AddDocument(2, "C# OOP");
Console.WriteLine("Пошук C#: " + string.Join(", ", index.Search("C#").Order()));
Console.WriteLine("Пошук C# AND LINQ: " + string.Join(", ", index.SearchMultiple("C#", "LINQ").Order()));

namespace IndependentWork9
{

public record FileMetadata(string FileName, long Size, DateTime LastModified, string Hash);
public class FileMetadataCache
{
    private readonly Dictionary<string, FileMetadata> _cache = new(StringComparer.OrdinalIgnoreCase);
    public void Add(string path, FileMetadata metadata) => _cache[path] = metadata;
    public FileMetadata Get(string path) => _cache.TryGetValue(path, out var value)
        ? value : throw new KeyNotFoundException($"Файл {path} не знайдено.");
    public bool Remove(string path) => _cache.Remove(path);
    public bool Contains(string path) => _cache.ContainsKey(path);
}

public class TagManager
{
    private readonly Dictionary<string, HashSet<string>> _articleTags = new();
    public void AddTag(string articleId, string tag)
    {
        if (!_articleTags.TryGetValue(articleId, out var tags))
            _articleTags[articleId] = tags = new(StringComparer.OrdinalIgnoreCase);
        tags.Add(tag);
    }
    public void RemoveTag(string articleId, string tag)
    {
        if (_articleTags.TryGetValue(articleId, out var tags)) tags.Remove(tag);
    }
    private HashSet<string> CopyTags(string id) => _articleTags.TryGetValue(id, out var tags)
        ? new(tags, StringComparer.OrdinalIgnoreCase) : new(StringComparer.OrdinalIgnoreCase);
    public HashSet<string> GetAllUniqueTags()
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var tags in _articleTags.Values) result.UnionWith(tags);
        return result;
    }
    public HashSet<string> FindCommonTags(string first, string second)
    {
        var result = CopyTags(first); result.IntersectWith(CopyTags(second)); return result;
    }
    public HashSet<string> FindExclusiveTags(string first, string second)
    {
        var result = CopyTags(first); result.ExceptWith(CopyTags(second)); return result;
    }
}

public class SearchIndex
{
    private readonly Dictionary<string, HashSet<int>> _invertedIndex = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<int, HashSet<string>> _documentWords = new();
    private static IEnumerable<string> Tokenize(string content) =>
        Regex.Matches(content, @"[\p{L}\p{N}]+(?:#|\+\+)?").Select(match => match.Value.ToLowerInvariant());
    public void AddDocument(int docId, string content)
    {
        var words = Tokenize(content).ToHashSet();
        if (_documentWords.TryGetValue(docId, out var oldWords))
            foreach (string word in oldWords)
            {
                _invertedIndex[word].Remove(docId);
                if (_invertedIndex[word].Count == 0) _invertedIndex.Remove(word);
            }
        _documentWords[docId] = words;
        foreach (string word in words)
        {
            if (!_invertedIndex.TryGetValue(word, out var ids)) _invertedIndex[word] = ids = new();
            ids.Add(docId);
        }
    }
    public HashSet<int> Search(string word)
    {
        var tokens = Tokenize(word).ToArray();
        return tokens.Length == 1 && _invertedIndex.TryGetValue(tokens[0], out var ids) ? new(ids) : new();
    }
    public HashSet<int> SearchMultiple(params string[] words)
    {
        var tokens = words.SelectMany(Tokenize).Distinct().ToArray();
        if (tokens.Length == 0) return new();
        var result = Search(tokens[0]);
        foreach (string word in tokens.Skip(1)) result.IntersectWith(Search(word));
        return result;
    }
}

}
