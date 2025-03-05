namespace ClickView.Extensions.Primitives.Tests.Extensions;

using Primitives.Extensions;
using Xunit;

public class EnumerableExtensionTests
{
    [Fact]
    public void TryGetFirstValue()
    {
        int[] list = [1, 2, 3];

        Assert.True(list.TryGetFirstValue(out var value));
        Assert.Equal(1, value);
    }
}
