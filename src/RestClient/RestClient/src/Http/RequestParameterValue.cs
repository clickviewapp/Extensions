namespace ClickView.Extensions.RestClient.Http
{
    internal class RequestParameterValue
    {
        public RequestParameterValue(object value, RequestParameterType type)
        {
            Value = value;
            Type = type;
        }

        internal object Value { get; set; }
        internal RequestParameterType Type { get; }
    }
}
