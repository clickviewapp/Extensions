namespace ClickView.Extensions.HealthCheck
{
    public class HealthCheckResultItem
    {
        internal HealthCheckResultItem(string name, HealthCheckResult result)
        {
            Name = name;
            Result = result;
            FormattedString = result.GetFormattedString();
        }

        public string Name { get; }
        public HealthCheckResult Result { get; }
        public string FormattedString { get; }
    }
}