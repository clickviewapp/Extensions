namespace ClickView.Extensions.RestClient.Authenticators.OAuth.Tokens
{
    public class RefreshToken : Token
    {
        public RefreshToken(string value) : base(TokenType.RefreshToken, value)
        {
        }
    }
}
