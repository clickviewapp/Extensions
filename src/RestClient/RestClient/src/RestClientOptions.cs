namespace ClickView.Extensions.RestClient
{
    using System;
    using System.Net;
    using Http;

    public class RestClientOptions : CoreRestClientOptions
    {
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
        public DecompressionMethods DecompressionMethods { get; set; } = DecompressionMethods.GZip;

        internal static readonly RestClientOptions Default = new RestClientOptions();
    }
}