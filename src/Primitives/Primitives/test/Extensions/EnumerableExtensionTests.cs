namespace ClickView.Extensions.Primitives.Tests.Extensions;

using Primitives.Extensions;
using Xunit;

public class EnumerableExtensionTests
{
    [Fact]
    public void TryGetFirstValue_Array()
    {
        int[] list = [1, 2, 3];

        Assert.True(list.TryGetFirstValue(out var value));
        Assert.Equal(1, value);
    }

    [Fact]
    public void TryGetFirstValue_Enumerable()
    {
        // Where() to force an Enumerable
        var list = Enumerable.Range(1, 100).Where(x => true);

        Assert.True(list.TryGetFirstValue(out var value));
        Assert.Equal(1, value);
    }

    [Fact]
    public void TryGetFirstValue_Empty()
    {
        int[] list = [];

        Assert.False(list.TryGetFirstValue(out _));
    }

    [Fact]
    public void TryGetFirstValue_EmptyEnumerable()
    {
        // Where() to force an Enumerable
        var list = Enumerable.Range(1, 100).Where(x => false);

        Assert.False(list.TryGetFirstValue(out _));
    }

    [Fact]
    public void WhereNotNull_ValueTypeListContainsNull_ReturnsNoNullValues()
    {
        int?[] list = [1, null, 2, null, 3];

        var result = list.WhereNotNull().ToList();

        Assert.All(result, i => Assert.NotNull(i));
        Assert.NotEqual(list.Length, result.Count);
    }

    [Fact]
    public void WhereNotNull_SelectorClassTypeListContainsNull_ReturnsNoNullValues()
    {
        Test[] list = [new("1"), new(null)];

        var result = list.WhereNotNull(x => x.Value).ToList();

        Assert.All(result, Assert.NotNull);
        Assert.NotEqual(list.Length, result.Count);
    }

    private class Test(string? value)
    {
        public string? Value { get; } = value;
    }
}
