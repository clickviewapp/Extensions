namespace ClickView.Extensions.RestClient.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.Encodings.Web;
    using Http;

    public static class QueryHelpers
    {
        /// <summary>
        ///     Append the given query key and value to the URI.
        /// </summary>
        /// <param name="uri">The base URI.</param>
        /// <param name="name">The name of the query key.</param>
        /// <param name="value">The query value.</param>
        /// <returns>The combined result.</returns>
        public static string AddQueryString(string uri, string name, string value)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return AddQueryString(uri, [new KeyValuePair<string, string?>(name, value)]);
        }

        /// <summary>
        ///     Append the given query keys and values to the uri.
        /// </summary>
        /// <param name="uri">The base uri.</param>
        /// <param name="queryString">A collection of name value query pairs to append.</param>
        /// <returns>The combined result.</returns>
        public static string AddQueryString(string uri, IDictionary<string, string?> queryString)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            if (queryString == null)
                throw new ArgumentNullException(nameof(queryString));

            return AddQueryString(uri, (IEnumerable<KeyValuePair<string, string?>>) queryString);
        }

        /// <summary>
        ///     Append the given query keys and values to the uri.
        /// </summary>
        /// <param name="uri">The base uri.</param>
        /// <param name="parameters">A collection of request parameters to append.</param>
        /// <returns>The combined result.</returns>
        internal static string AddQueryString(string uri, IDictionary<string, List<RequestParameterValue>> parameters)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            return AddQueryString(uri, Flatten(parameters));
        }

        private static IEnumerable<KeyValuePair<string, string?>> Flatten(IDictionary<string, List<RequestParameterValue>> parameters)
        {
            foreach (var p in parameters)
            foreach (var pp in p.Value.Where(v => v.Type == RequestParameterType.Query))
            {
                yield return new KeyValuePair<string, string?>(p.Key, (string?) pp.Value);
            }
        }

        private static string AddQueryString(string uri, IEnumerable<KeyValuePair<string, string?>> queryString)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            if (queryString == null)
                throw new ArgumentNullException(nameof(queryString));

            var anchorIndex = uri.IndexOf('#');
            var uriToBeAppended = uri;
            var anchorText = "";

            // If there is an anchor, then the query string must be inserted before its first occurrence.
            if (anchorIndex != -1)
            {
                anchorText = uri.Substring(anchorIndex);
                uriToBeAppended = uri.Substring(0, anchorIndex);
            }

            var queryIndex = uriToBeAppended.IndexOf('?');
            var hasQuery = queryIndex != -1;

            var sb = new StringBuilder();
            sb.Append(uriToBeAppended);

            foreach (var parameter in queryString)
            {
                if (parameter.Value == null)
                    continue;

                sb.Append(hasQuery ? '&' : '?');
                sb.Append(UrlEncoder.Default.Encode(parameter.Key));
                sb.Append('=');
                sb.Append(UrlEncoder.Default.Encode(parameter.Value));
                hasQuery = true;
            }

            sb.Append(anchorText);
            return sb.ToString();
        }
    }
}
