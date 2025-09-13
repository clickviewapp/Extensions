namespace ClickView.Extensions.RestClient.Tests;

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ClickView.Extensions.RestClient.Serialization;
using Requests;
using Xunit;

public class RestClientRequestTests
{
    [Fact]
    public void GetContent_NoBody_ReturnsNull()
    {
        var request = new RestClientRequest(HttpMethod.Post, "test");
        Assert.Null(request.GetContent());
    }

    [Fact]
    public async Task GetContent_StreamBody_ReturnsStreamContent()
    {
        var request = new RestClientRequest(HttpMethod.Post, "test");
        request.AddBody(new MemoryStream([1,2,3]));

        var result = request.GetContent();

        var streamContent = Assert.IsType<StreamContent>(result);
        var bytes = await streamContent.ReadAsByteArrayAsync();
        Assert.Equal(streamContent.Headers.ContentType, MediaTypeHeaderValue.Parse("application/octet-stream"));
        Assert.Equal([1, 2, 3], bytes);
    }

    [Fact]
    public async Task GetContent_StreamBodyContentType_ReturnsStreamContent()
    {
        var request = new RestClientRequest(HttpMethod.Post, "test");
        request.AddBody(new MemoryStream([1, 2, 3]), new MediaTypeHeaderValue("application/test"));

        var result = request.GetContent();

        var streamContent = Assert.IsType<StreamContent>(result);
        var bytes = await streamContent.ReadAsByteArrayAsync();
        Assert.Equal(streamContent.Headers.ContentType, MediaTypeHeaderValue.Parse("application/test"));
        Assert.Equal([1, 2, 3], bytes);
    }

    [Fact]
    public async Task GetContent_ObjectBody_ReturnsStringContent()
    {
        var request = new RestClientRequest(HttpMethod.Post, "test")
        {
            Serializer = new TestSerializer()
        };
        request.AddBody("hello world");

        var result = request.GetContent();

        var streamContent = Assert.IsType<StringContent>(result);
        var resultString = await streamContent.ReadAsStringAsync();

        Assert.Equal("hello world", resultString);
        Assert.Equal("application/test", streamContent.Headers.ContentType?.MediaType);
    }

    [Fact]
    public void AddBody_GetRequest_ThrowsException()
    {
        var request = new RestClientRequest(HttpMethod.Get, "test");

        var ex = Assert.Throws<Exception>(() => request.AddBody(1));
        Assert.Equal("Cannot add body to GET", ex.Message);
    }

    private class TestSerializer : ISerializer
    {
        public string Format => "test";

        public string Serialize(object obj)
        {
            return obj.ToString() ?? "";
        }

        public T? Deserialize<T>(string input)
        {
            throw new NotImplementedException();
        }

        public object? Deserialize(string input, Type type)
        {
            throw new NotImplementedException();
        }

#if NET
        public ValueTask<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
#else
        public Task<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
#endif
        {
            throw new NotImplementedException();
        }
    }
}
