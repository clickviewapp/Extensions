namespace ClickView.Extensions.RestClient.Requests
{
    using System.Collections.Generic;
    using System.Net.Http;

    public interface IClientRequest
    {
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; }
        string Resource { get; }
        HttpMethod Method { get; }

        void AddHeader(string key, string value);
        void AddHeader(string key, IEnumerable<string> values);
        void AddBody(object body);
    }
}