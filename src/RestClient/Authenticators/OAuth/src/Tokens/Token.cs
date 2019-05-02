namespace ClickView.Extensions.RestClient.Authenticators.OAuth.Tokens
{
    using System;

    public class Token
    {
        public TokenType TokenType { get; }
        public string Value { get; }
        public DateTimeOffset? ExpireTime { get; set; }

        public Token(TokenType tokenType, string value)
        {
            TokenType = tokenType;
            Value = value;
        }
    }
}