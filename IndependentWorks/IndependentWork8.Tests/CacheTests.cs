using IndependentWork7;
using Xunit;

public class CacheTests
{
    [Fact] public void NewCacheIsEmpty() => Assert.Equal(0, new Cache<int, string>(2).Count);
    [Fact] public void AddStoresValue()
    {
        var cache = new Cache<int, string>(2);
        cache.Add(1, "one");
        Assert.Equal("one", cache.Get(1));
        Assert.Equal(1, cache.Count);
    }
    [Fact] public void ContainsRecognizesPresentAndMissingKeys()
    {
        var cache = new Cache<int, int>(2);
        cache.Add(1, 10);
        Assert.True(cache.ContainsKey(1));
        Assert.False(cache.ContainsKey(2));
    }
    [Fact] public void MissingKeyThrows() => Assert.Throws<KeyNotFoundException>(() => new Cache<int, int>(1).Get(5));
    [Fact] public void OverflowEvictsOldestEvenAfterReadingIt()
    {
        var cache = new Cache<int, string>(2);
        cache.Add(1, "one"); cache.Add(2, "two"); cache.Get(1); cache.Add(3, "three");
        Assert.False(cache.ContainsKey(1));
        Assert.Equal("two", cache.Get(2)); Assert.Equal("three", cache.Get(3));
        Assert.Equal(2, cache.Count);
    }
    [Fact] public void UpdatingExistingKeyPreservesFifoAndCount()
    {
        var cache = new Cache<int, string>(2);
        cache.Add(1, "old"); cache.Add(2, "two"); cache.Add(1, "updated");
        Assert.Equal("updated", cache.Get(1)); Assert.Equal(2, cache.Count);
        cache.Add(3, "three");
        Assert.False(cache.ContainsKey(1)); Assert.True(cache.ContainsKey(2));
    }
    [Fact] public void RepeatedEvictionsKeepQueueConsistent()
    {
        var cache = new Cache<int, int>(2);
        for (int i = 0; i < 20; i++) cache.Add(i, i);
        Assert.Equal(2, cache.Count); Assert.Equal(18, cache.Get(18)); Assert.Equal(19, cache.Get(19));
        Assert.False(cache.ContainsKey(17));
    }
    [Fact] public void CapacityOneReplacesPreviousItem()
    {
        var cache = new Cache<int, int>(1); cache.Add(1, 10); cache.Add(2, 20);
        Assert.False(cache.ContainsKey(1)); Assert.Equal(20, cache.Get(2)); Assert.Equal(1, cache.Count);
    }
    [Theory] [InlineData(0)] [InlineData(-1)]
    public void NonPositiveCapacityThrows(int capacity) => Assert.Throws<ArgumentOutOfRangeException>(() => new Cache<int, int>(capacity));
}
