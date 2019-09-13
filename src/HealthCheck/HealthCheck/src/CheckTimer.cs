namespace ClickView.Extensions.HealthCheck
{
    using System;
    using System.Diagnostics;

    public class CheckTimer
    {
        private readonly Stopwatch _stopwatch;

        private CheckTimer()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public static CheckTimer Start()
        {
            return new CheckTimer();
        }

        public TimeSpan Stop()
        {
            _stopwatch.Stop();

            return _stopwatch.Elapsed;
        }
    }
}