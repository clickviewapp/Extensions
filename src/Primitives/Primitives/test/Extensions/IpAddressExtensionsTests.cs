namespace ClickView.Extensions.Primitives.Tests.Extensions;

using System.Net;
using Primitives.Extensions;
using Xunit;

// Thanks https://gist.github.com/angularsen/f77b53ee9966fcd914025e25a2b3a085
public class IpAddressExtensionsTests
{
    [Theory]
    [InlineData("1.1.1.1")] // Cloudflare DNS
    [InlineData("8.8.8.8")] // Google DNS
    [InlineData("20.112.52.29")] // microsoft.com
    public void IsPrivate_PublicIPv4_ReturnsFalse(string ip)
    {
        var ipAddress = IPAddress.Parse(ip);

        Assert.False(ipAddress.IsPrivate());
    }

    [Theory]
    [InlineData("::ffff:1.1.1.1")] // Cloudflare DNS
    [InlineData("::ffff:8.8.8.8")] // Google DNS
    [InlineData("::ffff:20.112.52.29")] // microsoft.com
    public void IsPrivate_PublicIPv4MappedToIPv6_ReturnsFalse(string ip)
    {
        var ipAddress = IPAddress.Parse(ip);

        Assert.False(ipAddress.IsPrivate());
    }

    [Theory]
    // Loopback IPv4 127.0.0.1 - 127.255.255.255 (127.0.0.0/8)
    [InlineData("127.0.0.1")]
    [InlineData("127.10.20.30")]
    [InlineData("127.255.255.255")]
    // Class A private IP 10.0.0.0 – 10.255.255.255 (10.0.0.0/8)
    [InlineData("10.0.0.0")]
    [InlineData("10.20.30.40")]
    [InlineData("10.255.255.255")]
    // Class B private IP 172.16.0.0 – 172.31.255.255 (172.16.0.0/12)
    [InlineData("172.16.0.0")]
    [InlineData("172.20.30.40")]
    [InlineData("172.31.255.255")]
    // Class C private IP 192.168.0.0 – 192.168.255.255 (192.168.0.0/16)
    [InlineData("192.168.0.0")]
    [InlineData("192.168.30.40")]
    [InlineData("192.168.255.255")]
    // Link local (169.254.x.x)
    [InlineData("169.254.0.0")]
    [InlineData("169.254.30.40")]
    [InlineData("169.254.255.255")]
    public void IsPrivate_PrivateIPv4_ReturnsTrue(string ip)
    {
        var ipAddress = IPAddress.Parse(ip);

        Assert.True(ipAddress.IsPrivate());
    }

    [Theory]
    // Loopback IPv4 127.0.0.1 - 127.255.255.254 (127.0.0.0/8)
    [InlineData("::ffff:127.0.0.1")]
    [InlineData("::ffff:127.10.20.30")]
    [InlineData("::ffff:127.255.255.254")]
    // Class A private IP 10.0.0.0 – 10.255.255.255 (10.0.0.0/8)
    [InlineData("::ffff:10.0.0.0")]
    [InlineData("::ffff:10.20.30.40")]
    [InlineData("::ffff:10.255.255.255")]
    // Class B private IP 172.16.0.0 – 172.31.255.255 (172.16.0.0/12)
    [InlineData("::ffff:172.16.0.0")]
    [InlineData("::ffff:172.20.30.40")]
    [InlineData("::ffff:172.31.255.255")]
    // Class C private IP 192.168.0.0 – 192.168.255.255 (192.168.0.0/16)
    [InlineData("::ffff:192.168.0.0")]
    [InlineData("::ffff:192.168.30.40")]
    [InlineData("::ffff:192.168.255.255")]
    // Link local (169.254.x.x)
    [InlineData("::ffff:169.254.0.0")]
    [InlineData("::ffff:169.254.30.40")]
    [InlineData("::ffff:169.254.255.255")]
    public void IsPrivate_PrivateIPv4MappedToIPv6_ReturnsTrue(string ip)
    {
        var ipAddress = IPAddress.Parse(ip);

        Assert.True(ipAddress.IsPrivate());
    }

    [Theory]
    [InlineData("::1")] // Loopback
    [InlineData("fe80::")] // Link local
    [InlineData("fe80:1234:5678::1")] // Link local
// Exclude these tests for NET6.0 and above as they dont work
#if NET
    [InlineData("fc00::")] // Unique local, globally assigned.
    [InlineData("fc00:1234:5678::1")] // Unique local, globally assigned.
    [InlineData("fd00::")] // Unique local, locally assigned.
    [InlineData("fd12:3456:789a::1")] // Unique local, locally assigned.
#endif
    public void IsPrivate_PrivateIPv6_ReturnsTrue(string ip)
    {
        var ipAddress = IPAddress.Parse(ip);

        Assert.True(ipAddress.IsPrivate());
    }

    [Theory]
    [InlineData("2606:4700:4700::64")] // Cloudflare DNS
    [InlineData("2001:4860:4860::8888")] // Google DNS
    [InlineData("2001:0db8:85a3:0000:0000:8a2e:0370:7334")] // Commonly used example.
    public void IsPrivate_PublicIPv6_ReturnsFalse(string ip)
    {
        var ipAddress = IPAddress.Parse(ip);

        Assert.False(ipAddress.IsPrivate());
    }
}
