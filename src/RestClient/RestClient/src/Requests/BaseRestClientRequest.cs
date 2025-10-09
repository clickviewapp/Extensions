namespace ClickView.Extensions.RestClient.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
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

    public abstract class BaseRestClientRequest<TResponse>(HttpMethod method, string resource) : IClientRequest
        where TResponse : RestClientResponse
    {
        private readonly RestClientRequestHeaders _headers = [];
        internal readonly Dictionary<string, List<RequestParameterValue>> Parameters = new();

        private object? _content;
        private MediaTypeHeaderValue? _contentType;

        public ISerializer Serializer { internal get; set; } = NewtonsoftJsonSerializer.Instance;

        /// <summary>
        ///     Set to true if we should throw an exception on 404
        /// </summary>
        protected bool ThrowOnNotFound { get; set; } = false;

        public string Resource { get; set; } = resource;
        public HttpMethod Method { get; } = method;
        public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers => _headers;

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

            _content = body;
        }

        public void AddBody(Stream stream, MediaTypeHeaderValue mediaType)
        {
            AddBody(stream);
            _contentType = mediaType;
        }

        public void AddQueryParameter(string key, string value)
        {
            if (!Parameters.TryGetValue(key, out var list))
            {
                list = [];
                Parameters.Add(key, list);
            }

            list.Add(new RequestParameterValue(value, RequestParameterType.Query));
        }

        public void AddQueryParameter(string key, IEnumerable<string> values)
        {
            if (!Parameters.TryGetValue(key, out var list))
            {
                list = [];
                Parameters.Add(key, list);
            }

            foreach (var value in values)
                list.Add(new RequestParameterValue(value, RequestParameterType.Query));
        }

        internal async Task<TResponse> GetResponseAsync(HttpResponseMessage message)
        {
            var response = await ParseResponseAsync(message).ConfigureAwait(false);
            var error = await GetErrorAsync(message).ConfigureAwait(false);

            if (error == null)
                return response;

            HandleError(error);
            response.Error = error;

            return response;
        }

        protected abstract Task<TResponse> ParseResponseAsync(HttpResponseMessage message);

        protected virtual bool TryParseErrorBody(string content,
#if NET
            [NotNullWhen(true)]
#endif
            out ErrorBody? error)
        {
            error = null;
            return false;
        }

        protected async Task<Error?> GetErrorAsync(HttpResponseMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            //There is no error
            if (message.IsSuccessStatusCode)
                return null;

            var contentString = await message.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (TryParseErrorBody(contentString, out var errorBody))
                return new Error(message.StatusCode, errorBody, contentString);

            // Make sure the error body is null
            errorBody = null;

            // We didnt parse an error body, so instead create a default one from the ReasonPhrase (if set)
            if (!string.IsNullOrEmpty(message.ReasonPhrase))
            {
                errorBody = new ErrorBody
                {
                    Message = message.ReasonPhrase
                };
            }

            return new Error(message.StatusCode, errorBody, contentString);
        }

        protected virtual void HandleError(Error error)
        {
            if (error.HttpStatusCode == HttpStatusCode.NotFound && !ThrowOnNotFound)
                return;

            var ex = ErrorHelper.GetErrorException(error);
            if (ex is not null)
                throw ex;
        }

        protected async ValueTask<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
        {
            try
            {
                return await Serializer.DeserializeAsync<T>(stream, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to deserialize response", ex);
            }
        }

        protected T? Deserialize<T>(string input)
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

        internal HttpContent? GetContent()
        {
            // If we have no content, return null
            if (_content is null)
                return null;

            // If our content is already a HttpContent, return that as is
            if (_content is Stream stream)
            {
                return new StreamContent(stream)
                {
                    Headers =
                    {
                        ContentType = _contentType ?? ContentTypes.OctetStream
                    }
                };
            }

            // Otherwise, serialize the content and return it

            //Check if we have a serializer
            if (Serializer == null)
                throw new InvalidOperationException("No serializer found");

            var mediaType = "application/" + Serializer.Format;

            string content;
            try
            {
                content = Serializer.Serialize(_content);
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to serialize request body", ex);
            }

            return new StringContent(content, Encoding.UTF8, mediaType);
        }
    }
}
