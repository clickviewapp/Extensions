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

        var result = list.WhereNotNull();

        Assert.Equal(result, [1, 2, 3]);
    }

    [Fact]
    public void WhereNotNull_SelectorClassTypeListContainsNull_ReturnsNoNullValues()
    {
        Test[] list = [new("1"), new(null), new("2"), new(null)];

        var result = list.WhereNotNull(x => x.Value);

        Assert.Equal(result, ["1", "2"]);
    }

    private class Test(string? value)
    {
        public string? Value { get; } = value;
    }
}
