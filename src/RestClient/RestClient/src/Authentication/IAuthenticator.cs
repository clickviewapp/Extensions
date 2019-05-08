namespace ClickView.Extensions.RestClient.Authentication
{
    using System.Threading.Tasks;
    using Requests;

    public interface IAuthenticator
    {
        Task AuthenticateAsync(IClientRequest request);
    }
}
