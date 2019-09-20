# HealthCheck

Contains libraries for performing health checks for resources used by your application.

## Usage

```cs
var builder = HealthCheck.CreateDefaultBuilder()
	.AddHttpCheck("Online", new Uri("https://example.com"))
	.AddRedisCheck("Redis", "localhost:6799")
	.AddMySqlCheck("MySql Repository", "server=192.168.0.1");

var checker = builder.Build();

await checker.CheckAllAsync(CancellationToken.None);
```
