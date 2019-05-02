namespace ClickView.Extensions.RestClient.Requests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Exceptions;
    using Helpers;
    using Http;
    using Responses;
    using Serialization;

    public abstract class BaseRestClientRequest<TResponse> : IClientRequest where TResponse : RestClientResponse
    {
        public string Resource { get; set; }
        public HttpMethod Method { get; }
        public ISerializer Serializer { internal get; set; } = NewonsoftJsonSerializer.Instance;
        public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers => _headers;

        internal HttpContent Content { get; set; }
        internal readonly Dictionary<string, List<RequestParameterValue>> Parameters = new Dictionary<string, List<RequestParameterValue>>();

        /// <summary>
        /// Set to true if we should throw an exception on 404
        /// </summary>
        protected bool ThrowOnNotFound { get; set; } = false;

        private readonly RestClientRequestHeaders _headers = new RestClientRequestHeaders();

        protected BaseRestClientRequest(HttpMethod method, string resource)
        {
            Method = method;
            Resource = resource;
        }

        public void AddHeader(string key, string value)
        {
            _headers.Add(key, value);
        }

        public void AddHeader(string key, IEnumerable<string> values)
        {
            _headers.Add(key, values);
        }

        public void AddBody(object body)
        {
            if (Method != HttpMethod.Post && Method != HttpMethod.Put)
                throw new Exception("Cannot add body to " + Method);

            //Check if we have a serializer
            if (Serializer == null)
                throw new ArgumentNullException(nameof(Serializer), "Serializer required");

            var mediaType = "application/" + Serializer.Format;

            string content;
            try
            {
                content = Serializer.Serialize(body);
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to serialize request body", ex);
            }

            Content = new StringContent(content, Encoding.UTF8, mediaType);
        }

        public void AddBody(Stream stream)
        {
            AddBody(stream, new MediaTypeHeaderValue("application/octet-stream"));
        }

        public void AddBody(Stream stream, MediaTypeHeaderValue mediaType)
        {
            Content = new StreamContent(stream)
            {
                Headers =
                {
                    ContentType = mediaType
                }
            };
        }

        public void AddQueryParameter(string key, string value)
        {
            if (!Parameters.TryGetValue(key, out var list))
            {
                list = new List<RequestParameterValue>();
                Parameters.Add(key, list);
            }

            list.Add(new RequestParameterValue(value, RequestParameterType.Query));
        }

        public void AddQueryParameter(string key, IEnumerable<string> values)
        {
            if (!Parameters.TryGetValue(key, out var list))
            {
                list = new List<RequestParameterValue>();
                Parameters.Add(key, list);
            }

            list.AddRange(values.Select(v => new RequestParameterValue(v, RequestParameterType.Query)));
        }

        internal async Task<TResponse> GetResponseAsync(HttpResponseMessage message)
        {
            var response = await ParseResponseAsync(message).ConfigureAwait(false);
            var error = await GetErrorAsync(message).ConfigureAwait(false);

            if (error != null)
            {
                HandleError(error);

                response.Error = error;
            }

            return response;
        }

        protected abstract Task<TResponse> ParseResponseAsync(HttpResponseMessage message);

        protected virtual bool TryParseErrorBody(string content, out ErrorBody error)
        {
            error = null;
            return false;
        }

        protected virtual ErrorBody GetDefaultErrorBody(HttpStatusCode httpStatusCode, string defaultMessage)
        {
            return new ErrorBody
            {
                Message = ErrorHelper.GetDefaultErrorMessage(httpStatusCode) ?? defaultMessage
            };
        }

        protected async Task<Error> GetErrorAsync(HttpResponseMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            //There is no error
            if (message.IsSuccessStatusCode)
                return null;

            if (message.Content != null)
            {
                var content = await message.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (TryParseErrorBody(content, out var errorBody))
                    return new Error(message.StatusCode, errorBody);
            }

            return new Error(message.StatusCode, GetDefaultErrorBody(message.StatusCode, message.ReasonPhrase));
        }

        protected virtual void HandleError(Error error)
        {
            if (error.HttpStatusCode == HttpStatusCode.NotFound && !ThrowOnNotFound)
                return;

            throw ErrorHelper.GetErrorException(error);
        }

        protected T Deserialize<T>(string input)
        {
            try
            {
                return Serializer.Deserialize<T>(input);
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to deserialize response", ex);
            }
        }
    }
}