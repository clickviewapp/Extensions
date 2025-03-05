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
        IEnumerable<int> list = Enumerable.Range(1, 100).Where(x => true);

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
        IEnumerable<int> list = Enumerable.Range(1, 100).Where(x => false);

        Assert.False(list.TryGetFirstValue(out _));
    }
}
