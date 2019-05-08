namespace ClickView.Extensions.RestClient
{
    using System;
    using System.Net;
    using Http;

    public class RestClientOptions : CoreRestClientOptions
    {
        internal static readonly RestClientOptions Default = new RestClientOptions();
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
        public DecompressionMethods DecompressionMethods { get; set; } = DecompressionMethods.GZip;
    }
}
