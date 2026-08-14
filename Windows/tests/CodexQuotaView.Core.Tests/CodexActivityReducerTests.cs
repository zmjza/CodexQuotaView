using CodexQuotaView.Core;
using Xunit;

namespace CodexQuotaView.Core.Tests;

public sealed class CodexActivityReducerTests
{
    private static CodexActivityEvent Event(
        CodexActivityHookEvent kind,
        CodexActivitySessionStartSource? source = null,
        CodexActivityToolCategory? tool = null)
    {
        return new CodexActivityEvent(
            CodexActivityReducer.CurrentSchemaVersion,
            kind,
            "session-hash",
            null,
            "repo",
            tool,
            source,
            DateTimeOffset.UtcNow);
    }

    [Fact]
    public void FullSessionSequenceMatchesMacOSStates()
    {
        Assert.Equal(
            (CodexActivityVisualState.Standby, CodexActivityOperationKey.ConnectingSession),
            SnapshotOf(Event(CodexActivityHookEvent.SessionStart)));
        Assert.Equal(
            (CodexActivityVisualState.Thinking, CodexActivityOperationKey.AnalyzingRequest),
            SnapshotOf(Event(CodexActivityHookEvent.UserPromptSubmit)));
        Assert.Equal(
            (CodexActivityVisualState.Working, CodexActivityOperationKey.ExecutingShell),
            SnapshotOf(Event(CodexActivityHookEvent.PreToolUse, tool: CodexActivityToolCategory.Shell)));
        Assert.Equal(
            (CodexActivityVisualState.AwaitingConfirmation, CodexActivityOperationKey.AwaitingApproval),
            SnapshotOf(Event(CodexActivityHookEvent.PermissionRequest)));
        Assert.Equal(
            (CodexActivityVisualState.Thinking, CodexActivityOperationKey.ReviewingToolResult),
            SnapshotOf(Event(CodexActivityHookEvent.PostToolUse)));
        Assert.Equal(
            (CodexActivityVisualState.Completed, CodexActivityOperationKey.TurnCompleted),
            SnapshotOf(Event(CodexActivityHookEvent.Stop)));
    }

    [Fact]
    public void CompactionCycleUsesCompactingStates()
    {
        Assert.Equal(
            (CodexActivityVisualState.CompactingContext, CodexActivityOperationKey.CompactingContext),
            SnapshotOf(Event(CodexActivityHookEvent.PreCompact)));
        Assert.Equal(
            (CodexActivityVisualState.Thinking, CodexActivityOperationKey.ContinuingAfterCompaction),
            SnapshotOf(Event(CodexActivityHookEvent.PostCompact)));
        Assert.Equal(
            (CodexActivityVisualState.Thinking, CodexActivityOperationKey.ContinuingAfterCompaction),
            SnapshotOf(Event(
                CodexActivityHookEvent.SessionStart,
                source: CodexActivitySessionStartSource.Compact)));
    }

    [Fact]
    public void InvalidOrSensitiveInputsDoNotProduceSnapshots()
    {
        Assert.Null(CodexActivityReducer.Reduce(null));
        var badVersion = Event(CodexActivityHookEvent.Stop) with { SchemaVersion = 99 };
        Assert.Null(CodexActivityReducer.Reduce(badVersion));
    }

    [Fact]
    public void InactivityAndHideRulesMatchMacOS()
    {
        Assert.True(CodexActivityReducer.ShouldStartInactivityCycle(Event(CodexActivityHookEvent.Stop)));
        Assert.True(CodexActivityReducer.ShouldHideImmediately(Event(CodexActivityHookEvent.SessionEnd)));
        Assert.False(CodexActivityReducer.ShouldHideImmediately(Event(CodexActivityHookEvent.Stop)));
    }

    private static (CodexActivityVisualState, CodexActivityOperationKey) SnapshotOf(CodexActivityEvent activityEvent)
    {
        var snapshot = CodexActivityReducer.Reduce(activityEvent);
        Assert.NotNull(snapshot);
        return (snapshot!.State, snapshot.OperationKey);
    }
}
