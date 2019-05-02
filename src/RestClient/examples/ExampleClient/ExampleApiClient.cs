namespace ExampleClient
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using ClickView.Extensions.RestClient;
    using ClickView.Extensions.RestClient.Authentication;
    using ClickView.Extensions.RestClient.Requests;

    public class ExampleApiClient
    {
        private readonly RestClient _client;

        public ExampleApiClient(Uri baseAddress, string authKey)
        {
            _client = new RestClient(baseAddress, new RestClientOptions
            {
                Authenticator = new HeaderAuthenticator("x-auth-header", authKey)
            });
        }

        /// <summary>
        /// Gets a user using a custom request object
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<UserModel> GetUserCustomRequestAsync(string username)
        {
            var response = await _client.ExecuteAsync(new ExampleUserRequest(username)).ConfigureAwait(false);

            return response.Data;
        }

        public async Task<UserModel> GetUserAsync(string username)
        {
            var request = new RestClientRequest<UserModel>(HttpMethod.Get, "v1/users");

            request.AddQueryParameter("username", username);

            var response = await _client.ExecuteAsync(request).ConfigureAwait(false);

            return response.Data;
        }
    }
}
