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
        private readonly string _encoded;

        public BasicAccessAuthenticator(string userId, string password)
        {
            if (userId is null) throw new ArgumentNullException(nameof(userId));
            if (password is null) throw new ArgumentNullException(nameof(password));

            _encoded = ToBase64($"{userId}:{password}");
        }

        public Task AuthenticateAsync(IClientRequest request, CancellationToken token = default)
        {
            request.AddHeader("Authorization", $"Basic {_encoded}");
            return Task.CompletedTask;
        }

        private static string ToBase64(string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            return Convert.ToBase64String(bytes);
        }
    }
}
