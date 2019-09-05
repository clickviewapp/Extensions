namespace ClickView.Extensions.RestClient.Authentication
{
    using System.Threading;
    using System.Threading.Tasks;
    using Requests;

    /// <summary>
    ///     Basic authenticator which adds a header value to each request
    /// </summary>
    public class HeaderAuthenticator : IAuthenticator
    {
        private readonly string _key;
        private readonly string _value;

        public HeaderAuthenticator(string key, string value)
        {
            _key = key;
            _value = value;
        }

        public Task AuthenticateAsync(IClientRequest request, CancellationToken token = default)
        {
            request.AddHeader(_key, _value);

            return Task.CompletedTask;
        }
    }
}
