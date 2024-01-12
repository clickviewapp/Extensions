namespace ClickView.Extensions.RestClient.Tests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using Moq.Protected;
    using Requests;
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
    }
}
