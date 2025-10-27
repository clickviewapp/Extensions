namespace ClickView.Extensions.RestClient.Tests
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Http;
    using Moq;
    using Moq.Protected;
    using Requests;
    using Responses;
    using Xunit;

    public class RestClientTests
    {
        [Fact]
        public async Task ExecuteAsync_UsesBaseAddress()
        {
            // setup message handler
            var mockMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(message =>
                        message.RequestUri!.Equals(new Uri("http://clickview.com.au/v1/test"))),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("test"),
                })
                .Verifiable();

            // arrange
            var request = new RestClientRequest(HttpMethod.Get, "v1/test");
            var client = new RestClient(new Uri("http://clickview.com.au"), new HttpClient(mockMessageHandler.Object));

            // act
            await client.ExecuteAsync(request);

            // assert
            mockMessageHandler.Verify();
        }

        [Fact]
        public async Task ExecuteAsync_UsesBaseAddressOverHttpClientBaseAddress()
        {
            // setup message handler
            var mockMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(message =>
                        message.RequestUri!.Equals(new Uri("http://clickview.com.au/v1/test"))),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("test"),
                })
                .Verifiable();

            // arrange
            var request = new RestClientRequest(HttpMethod.Get, "v1/test");
            var client = new RestClient(new Uri("http://clickview.com.au"), new HttpClient(mockMessageHandler.Object)
            {
                BaseAddress = new Uri("http://clickview.co.uk")
            });

            // act
            await client.ExecuteAsync(request);

            // assert
            mockMessageHandler.Verify();
        }

        [Fact]
        public async Task ExecuteAsync_UsesHttpClientBaseAddress()
        {
            // setup message handler
            var mockMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(message =>
                        message.RequestUri!.Equals(new Uri("https://hello.hello/test"))),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("test"),
                })
                .Verifiable();

            // arrange
            var request = new RestClientRequest(HttpMethod.Get, "test");
            var client = new RestClient(new HttpClient(mockMessageHandler.Object)
            {
                BaseAddress = new Uri("https://hello.hello")
            });

            // act
            await client.ExecuteAsync(request);

            // assert
            mockMessageHandler.Verify();
        }

        [Fact]
        public async Task ExecuteAsync_AbsoluteUriRequest()
        {
            // setup message handler
            var mockMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(message =>
                        message.RequestUri!.Equals(new Uri("https://clockview.clocks/test"))),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("test"),
                })
                .Verifiable();

            // arrange
            var request = new RestClientRequest(HttpMethod.Get, "https://clockview.clocks/test");
            var client = new RestClient(new HttpClient(mockMessageHandler.Object)
            {
                BaseAddress = new Uri("https://hello.hello")
            });

            // act
            await client.ExecuteAsync(request);

            // assert
            mockMessageHandler.Verify();
        }

        [Fact]
        public async Task ExecuteAsync_NoBaseAddress_ThrowsInvalidOperationException()
        {
            // setup message handler
            var mockMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            // arrange
            var request = new RestClientRequest(HttpMethod.Get, "test");
            var client = new RestClient(new HttpClient(mockMessageHandler.Object));

            // act/assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => client.ExecuteAsync(request));
        }

        [Fact]
        public async Task ExecuteAsync_AddBodyStream_MessageContentStreamContent()
        {
            // setup
            var mockMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(message => message.Content!.GetType() == typeof(StreamContent)),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("test: success"),
                })
                .Verifiable();

            var request = new TestRequest(HttpMethod.Post, "http://localhost/test");
            var client = new RestClient(new HttpClient(mockMessageHandler.Object));

            request.AddBody(new MemoryStream());

            var response = await client.ExecuteAsync(request);

            mockMessageHandler.Verify();
            Assert.Equal("test: success", response.Data);
        }

        [Fact]
        public void RestClient_DefaultUserAgentSet_DoesNotThrow()
        {
            _ = new RestClient(new Uri("http://localhost"), new RestClientOptions
            {
                DefaultUserAgent = "testapp/1.0.0 (example; test/2.0.0)"
            });
        }

        [Fact]
        public async Task ExecuteAsync_DefaultUserAgentSet_RequestContainsDefaultUserAgent()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            string? userAgentHeader = null;

            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Callback<HttpRequestMessage, CancellationToken>((request, token) =>
                {
                    userAgentHeader = request.Headers.UserAgent.ToString();
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("test: success")
                })
                .Verifiable();

            var request = new TestRequest(HttpMethod.Post, "http://localhost/test");
            var client = new RestClient(new HttpClient(mockMessageHandler.Object), new CoreRestClientOptions
            {
                DefaultUserAgent = "testapp/1.0.0 (rv:1; test; example/1.2.3)"
            });

            await client.ExecuteAsync(request);

            mockMessageHandler.Verify();
            Assert.Equal("testapp/1.0.0 (rv:1; test; example/1.2.3)", userAgentHeader);
        }

        private class TestRequest(HttpMethod method, string resource) : RestClientRequest<string>(method, resource)
        {
            protected override async ValueTask<RestClientResponse<string>> ParseResponseAsync(HttpResponseMessage message)
            {
                return new RestClientResponse<string>(message, await message.Content.ReadAsStringAsync());
            }
        }
    }
}
