namespace ClickView.Extensions.RestClient.Serialization
{
    using System;
    using System.Globalization;
    using System.Text;
    using Newtonsoft.Json;

    public class NewtonsoftJsonSerializer : ISerializer
    {
        public static readonly ISerializer Instance = new NewtonsoftJsonSerializer();

        private readonly JsonSerializer _serializer;

        public NewtonsoftJsonSerializer()
        {
            //use default
            _serializer = JsonSerializer.Create();

            // Seems that by default DeserializeObject should check for additional content
            // see: https://github.com/JamesNK/Newtonsoft.Json/blob/96df2ab9c15334f5bf2bf530a6bc1d9cb33f0c77/Src/Newtonsoft.Json/JsonConvert.cs#L862
            _serializer.CheckAdditionalContent = true;
        }

        public NewtonsoftJsonSerializer(JsonSerializerSettings jsonSerializerSettings)
        {
            _serializer = JsonSerializer.Create(jsonSerializerSettings);

            // Seems that by default DeserializeObject should check for additional content
            // see: https://github.com/JamesNK/Newtonsoft.Json/blob/96df2ab9c15334f5bf2bf530a6bc1d9cb33f0c77/Src/Newtonsoft.Json/JsonConvert.cs#L862
            _serializer.CheckAdditionalContent = true;
        }

        public string Format => "json";

        public string Serialize(object obj)
        {
            var sb = new StringBuilder(256);
            var sw = new StringWriter(sb, CultureInfo.InvariantCulture);
            using (var jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = _serializer.Formatting;

                _serializer.Serialize(jsonWriter, obj);
            }

            return sw.ToString();
        }

        public T? Deserialize<T>(string input)
        {
            return (T?) Deserialize(input, typeof(T));
        }

        public object? Deserialize(string input, Type type)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            using var reader = new JsonTextReader(new StringReader(input));

            return _serializer.Deserialize(reader, type);
        }

        public ValueTask<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            using var jsonReader = new JsonTextReader(new StreamReader(stream));

            var result = _serializer.Deserialize<T>(jsonReader);

            return new ValueTask<T?>(result);
        }
    }
}
