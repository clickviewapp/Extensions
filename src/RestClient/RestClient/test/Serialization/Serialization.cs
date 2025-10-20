namespace ClickView.Extensions.RestClient.Tests.Serialization;

using ClickView.Extensions.RestClient.Serialization;
using Xunit;

public class NewtonsoftJsonSerializerTests
{
    [Fact]
    public void Deserialize_Null_Throws()
    {
        var serializer = new NewtonsoftJsonSerializer();

        Assert.Throws<ArgumentNullException>(() => serializer.Deserialize(null!, typeof(string)));
    }

    [Fact]
    public void DeserializeTypeT_Null_Throws()
    {
        var serializer = new NewtonsoftJsonSerializer();

        Assert.Throws<ArgumentNullException>(() => serializer.Deserialize<string>(null!));
    }

    [Fact]
    public void Deserialize_EmptyString_ReturnsNull()
    {
        var serializer = new NewtonsoftJsonSerializer();

        var result = serializer.Deserialize(string.Empty, typeof(string));

        Assert.Null(result);
    }

    [Fact]
    public void DeserializeTypeT_EmptyString_ReturnsNull()
    {
        var serializer = new NewtonsoftJsonSerializer();

        var result = serializer.Deserialize<string>(string.Empty);

        Assert.Null(result);
    }

    [Fact]
    public void Deserialize_StringNullText_ReturnsNull()
    {
        var serializer = new NewtonsoftJsonSerializer();

        var result = serializer.Deserialize("null", typeof(string));

        Assert.Null(result);
    }

    [Fact]
    public void DeserializeTypeT_StringNullText_ReturnsNull()
    {
        var serializer = new NewtonsoftJsonSerializer();

        var result = serializer.Deserialize<string>("null");

        Assert.Null(result);
    }
}
