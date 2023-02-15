namespace ClickView.Extensions.RestClient.Authenticators.OAuth.Tokens
{
    using System;

    public class RefreshToken : Token
    {
        public RefreshToken(string value, DateTimeOffset expireTime) : base(TokenType.RefreshToken, value)
        {
            ExpireTime = expireTime;
        }
    }
}
