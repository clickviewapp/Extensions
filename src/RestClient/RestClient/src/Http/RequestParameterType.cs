namespace ClickView.Extensions.RestClient.Http
{
    using System;

    [Flags]
    public enum RequestParameterType
    {
        Query,
        Body
    }
}