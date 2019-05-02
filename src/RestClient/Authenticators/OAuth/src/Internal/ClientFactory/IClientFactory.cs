namespace ClickView.Extensions.RestClient.Authenticators.OAuth.Internal.ClientFactory
{
    using System.Threading.Tasks;
    using IdentityModel.Client;

    public interface IClientFactory
    {
        Task<TokenClient> GetTokenClientAsync();
    }
}