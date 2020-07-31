namespace ClickView.Extensions.RestClient.Authenticators.OAuth.Endpoints
{
    using System.Threading.Tasks;

    public interface IAuthenticatorEndpointFactory
    {
        Task<AuthenticatorEndpoints> GetAsync();
    }
}