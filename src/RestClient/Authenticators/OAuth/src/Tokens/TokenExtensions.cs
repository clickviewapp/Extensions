namespace ClickView.Extensions.RestClient.Authenticators.OAuth.Tokens
{
    using System;

    public static class TokenExtensions
    {
        public static bool IsValid(this Token token)
        {
            if (token == null)
                return false;

            if (token.ExpireTime.HasValue)
                return token.ExpireTime.Value > DateTimeOffset.UtcNow;

            return true;
        }
    }
}
