namespace ClickView.Extensions.RestClient
{
    using System;
    using System.Net;
    using Http;

    public class RestClientOptions : CoreRestClientOptions
    {
        internal static readonly RestClientOptions Default = new();
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
        public DecompressionMethods DecompressionMethods { get; set; } = DecompressionMethods.GZip;

#if NET
        public TimeSpan ConnectionLifetime { get; set; } = TimeSpan.FromMinutes(5);
#endif
    }
}
