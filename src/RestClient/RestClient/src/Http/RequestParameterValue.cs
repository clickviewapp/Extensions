namespace ClickView.Extensions.RestClient.Http
{
    internal class RequestParameterValue
    {
        internal object Value { get; set; }
        internal RequestParameterType Type { get; }

        public RequestParameterValue(object value, RequestParameterType type)
        {
            Value = value;
            Type = type;
        }
    }
}