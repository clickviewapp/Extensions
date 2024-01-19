namespace ClickView.Extensions.RestClient.Serialization
{
    using System;
    using Newtonsoft.Json;

    public class NewtonsoftJsonSerializer : ISerializer
    {
        public static readonly ISerializer Instance = new NewtonsoftJsonSerializer();

        private readonly JsonSerializerSettings? _jsonSerializerSettings;

        public NewtonsoftJsonSerializer()
        {
            //use default
            _jsonSerializerSettings = null;
        }

        public NewtonsoftJsonSerializer(JsonSerializerSettings jsonSerializerSettings)
        {
            _jsonSerializerSettings = jsonSerializerSettings;
        }

        public string Format => "json";

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, _jsonSerializerSettings);
        }

        public T? Deserialize<T>(string input)
        {
            return JsonConvert.DeserializeObject<T>(input, _jsonSerializerSettings);
        }

        public object? Deserialize(string input, Type type)
        {
            return JsonConvert.DeserializeObject(input, type, _jsonSerializerSettings);
        }
    }
}
