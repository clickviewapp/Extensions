namespace ClickView.Extensions.RestClient.Serialization;

using System.Text.Json;
using System.Text.Json.Serialization;

public class SystemTextJsonSerializer(JsonSerializerOptions options) : ISerializer
{
    public static readonly ISerializer Default = new SystemTextJsonSerializer();

    public SystemTextJsonSerializer() : this(GetDefaultOptions())
    {
    }

    public static JsonSerializerOptions GetDefaultOptions()
    {
        return new JsonSerializerOptions
        {
            // Same defaults as JsonSerializerDefaults.Web
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };
    }

    public string Serialize(object obj)
    {
        return JsonSerializer.Serialize(obj, options);
    }

    public T? Deserialize<T>(string input)
    {
        if (input is null)
            throw new ArgumentNullException(nameof(input));

        return JsonSerializer.Deserialize<T>(input, options)!;
    }

    public object? Deserialize(string input, Type type)
    {
        if (input is null)
            throw new ArgumentNullException(nameof(input));

        return JsonSerializer.Deserialize(input, type, options)!;
    }

    public ValueTask<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
    {
        return JsonSerializer.DeserializeAsync<T>(stream, options, cancellationToken);
    }

    public string Format => "json";
}
