namespace ClickView.Extensions.HealthCheck.Http
{
    public sealed class HeartbeatResponseMessage
    {
        public string Message { get; }

        private HeartbeatResponseMessage(string message)
        {
            Message = message;
        }

        public static HeartbeatResponseMessage Ok()
        {
            return new HeartbeatResponseMessage("Everything's coming up Milhouse!");
        }

        public static HeartbeatResponseMessage NotOk()
        {
            return new HeartbeatResponseMessage("Uh-oh");
        }
    }
}
