namespace ClickView.Extensions.RestClient.Authentication
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Requests;

    /// <summary>
    ///     Basic Access Authenticator which adds an Authorization header value to each request
    ///     Follows this spec: https://tools.ietf.org/html/rfc7617
    /// </summary>
    public class BasicAccessAuthenticator : IAuthenticator
    {
        private readonly string _key;
        private readonly string _val;

        public BasicAccessAuthenticator(string key, string val)
        {
            _key = key;
            _val = val;
        }

        public Task AuthenticateAsync(IClientRequest request, CancellationToken token = default)
        {
            var encoded = ToBase64($"{_key}:{_val}");
            request.AddHeader("Authorization", $"Basic {encoded}");
            return Task.CompletedTask;
        }

        private static string ToBase64(string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            return Convert.ToBase64String(bytes);
        }
    }
}