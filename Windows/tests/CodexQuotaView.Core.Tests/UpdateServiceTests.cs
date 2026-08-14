using CodexQuotaView.Core;
using Xunit;

namespace CodexQuotaView.Core.Tests;

public sealed class UpdateServiceTests
{
    [Theory]
    [InlineData("1.0.1", true)]
    [InlineData("1.0.0", false)]
    [InlineData("0.9.9", false)]
    [InlineData("not-a-version", false)]
    public void NewerVersionDetectionMatchesPolicy(string candidate, bool expected)
    {
        Assert.Equal(expected, UpdateService.IsNewerThanCurrent(candidate));
    }

    [Fact]
    public void VersionParsingHandlesBuildSuffix()
    {
        Assert.True(UpdateService.TryParseVersion("1.0.1-build.2", out var parsed));
        Assert.Equal("1.0.1", parsed.Version);
        Assert.Equal(2, parsed.Build);
    }

    [Fact]
    public void ReleaseValidationRequiresStableChannelAndHttpUrl()
    {
        var valid = new UpdateReleaseInfo("1.0.1", "stable", "https://github.com/zmjza/CodexQuotaView/releases/download/v1.0.1/app.zip", 1024);
        Assert.True(UpdateService.IsValidRelease(valid));

        var wrongChannel = valid with { Channel = "beta" };
        Assert.False(UpdateService.IsValidRelease(wrongChannel));

        var badUrl = valid with { DownloadUrl = "not-a-url" };
        Assert.False(UpdateService.IsValidRelease(badUrl));
    }
}
