using CodexQuotaView.Core;
using Xunit;

namespace CodexQuotaView.Core.Tests;

public sealed class HookEventSanitizerTests
{
    [Fact]
    public void HashIsDeterministicAndOpaque()
    {
        var first = HookEventSanitizer.HashIdentifier("session-abc");
        var second = HookEventSanitizer.HashIdentifier("session-abc");
        Assert.Equal(first, second);
        Assert.Equal(64, first.Length);
        Assert.DoesNotContain("session-abc", first);
    }

    [Fact]
    public void WorkspaceNameKeepsOnlyLeaf()
    {
        Assert.Equal("my-project", HookEventSanitizer.SanitizeWorkspaceName(@"C:\Users\me\projects\my-project"));
        Assert.Null(HookEventSanitizer.SanitizeWorkspaceName(null));
        Assert.Null(HookEventSanitizer.SanitizeWorkspaceName("   "));
    }

    [Fact]
    public void SensitiveFieldsRejectPayload()
    {
        var json = """
        {
          "event": "UserPromptSubmit",
          "prompt": "delete everything",
          "session_id": "abc"
        }
        """;
        using var result = HookEventSanitizer.SanitizePayload(json, out var reason);
        Assert.Null(result);
        Assert.Contains("sensitive", reason ?? string.Empty);
    }

    [Fact]
    public void AllowedFieldsSurviveAndOthersAreDropped()
    {
        var json = """
        {
          "event": "PostToolUse",
          "session_id": "abc",
          "tool_name": "Bash",
          "workspace_name": "repo",
          "occurred_at": "2026-08-14T08:00:00Z"
        }
        """;
        using var result = HookEventSanitizer.SanitizePayload(json, out var reason);
        Assert.NotNull(result);
        Assert.Null(reason);
        var root = result!.RootElement;
        Assert.True(root.TryGetProperty("event", out _));
        Assert.True(root.TryGetProperty("workspace_name", out _));
        Assert.False(root.TryGetProperty("session_id", out _));
        Assert.False(root.TryGetProperty("tool_name", out _));
    }
}
