namespace ClickView.Extensions.RestClient.Authenticators.OAuth.Tokens
{
    using System;

    public class AccessToken : Token
    {
        public AccessToken(string value, DateTimeOffset expireTime) : base(TokenType.AccessToken, value)
        {
            ExpireTime = expireTime;
        }
    }
}
