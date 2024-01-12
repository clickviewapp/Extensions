namespace ClickView.Extensions.RestClient.Serialization
{
    using System;

    public interface ISerializer
    {
        string Format { get; }

        string Serialize(object obj);

        T? Deserialize<T>(string input);
        object? Deserialize(string input, Type type);
    }
}
