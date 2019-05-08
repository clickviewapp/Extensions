namespace ExampleClient
{
    using System.Net;
    using System.Net.Http;
    using ClickView.Extensions.RestClient;
    using ClickView.Extensions.RestClient.Requests;

    /// <summary>
    ///Example request with custom error response parsing
    /// </summary>
    public class ExampleUserRequest : RestClientRequest<UserModel>
    {
        public ExampleUserRequest(string username) : base(HttpMethod.Get, "v1/users")
        {
            //Throw exceptions on 404
            ThrowOnNotFound = true;

            AddQueryParameter("username", username);
        }

        protected override void HandleError(Error error)
        {
            if (error.HttpStatusCode == HttpStatusCode.BadRequest)
                return;

            base.HandleError(error);
        }

        protected override bool TryParseErrorBody(string content, out ErrorBody error)
        {
            var errorResponse = Deserialize<ErrorResponse>(content);

            if (errorResponse == null)
            {
                error = null;
                return false;
            }

            error = new ErrorBody
            {
                Message = errorResponse.Message
            };

            return true;
        }
    }
}