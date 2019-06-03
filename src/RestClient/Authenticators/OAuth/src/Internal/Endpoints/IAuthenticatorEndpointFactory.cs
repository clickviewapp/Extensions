namespace ClickView.Extensions.RestClient.Authenticators.OAuth.Internal.Endpoints
{
    using System.Threading.Tasks;

    public interface IAuthenticatorEndpointFactory
    {
        Task<AuthenticatorEndpoints> GetAsync();
    }
}