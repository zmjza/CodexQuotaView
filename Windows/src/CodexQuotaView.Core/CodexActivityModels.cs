namespace CodexQuotaView.Core;

public enum CodexActivityHookEvent
{
    SessionStart,
    SessionEnd,
    UserPromptSubmit,
    PreToolUse,
    PermissionRequest,
    PostToolUse,
    PreCompact,
    PostCompact,
    SubagentStart,
    SubagentStop,
    Stop,
}

public enum CodexActivityToolCategory
{
    Shell,
    FileEdit,
    Mcp,
    Subagent,
    LocalTool,
    Unknown,
}

public enum CodexActivitySessionStartSource
{
    Startup,
    Resume,
    Clear,
    Compact,
}

public enum CodexActivityVisualState
{
    DisconnectedCodex,
    Standby,
    Thinking,
    Working,
    CompactingContext,
    AwaitingConfirmation,
    Completed,
    Error,
    Unavailable,
}

public enum CodexActivityOperationKey
{
    ConnectingSession,
    SessionEnded,
    AnalyzingRequest,
    ExecutingShell,
    EditingFiles,
    CallingExternalTool,
    CoordinatingSubagent,
    UsingLocalTool,
    UsingTool,
    AwaitingApproval,
    ReviewingToolResult,
    CompactingContext,
    ContinuingAfterCompaction,
    SubagentStarted,
    SubagentStopped,
    TurnCompleted,
    BridgeUnavailable,
    MalformedEvent,
}

public sealed record CodexActivityEvent(
    int SchemaVersion,
    CodexActivityHookEvent Event,
    string SessionHash,
    string? TurnHash,
    string? WorkspaceName,
    CodexActivityToolCategory? ToolCategory,
    CodexActivitySessionStartSource? SessionStartSource,
    DateTimeOffset OccurredAt);

public sealed record CodexActivitySnapshot(
    string SessionHash,
    CodexActivityVisualState State,
    string? WorkspaceName,
    CodexActivityOperationKey OperationKey,
    CodexActivityToolCategory? ToolCategory,
    DateTimeOffset OccurredAt);

public static class CodexActivityReducer
{
    public const int CurrentSchemaVersion = 1;

    public static CodexActivitySnapshot? Reduce(CodexActivityEvent? event)
    {
        if (event is null || event.SchemaVersion != CurrentSchemaVersion || string.IsNullOrEmpty(event.SessionHash))
        {
            return null;
        }

        var (state, operation) = event.Event switch
        {
            CodexActivityHookEvent.SessionStart => event.SessionStartSource == CodexActivitySessionStartSource.Compact
                ? (CodexActivityVisualState.Thinking, CodexActivityOperationKey.ContinuingAfterCompaction)
                : (CodexActivityVisualState.Standby, CodexActivityOperationKey.ConnectingSession),
            CodexActivityHookEvent.SessionEnd => (CodexActivityVisualState.Standby, CodexActivityOperationKey.SessionEnded),
            CodexActivityHookEvent.UserPromptSubmit => (CodexActivityVisualState.Thinking, CodexActivityOperationKey.AnalyzingRequest),
            CodexActivityHookEvent.PreToolUse => (CodexActivityVisualState.Working, OperationForTool(event.ToolCategory)),
            CodexActivityHookEvent.PermissionRequest => (CodexActivityVisualState.AwaitingConfirmation, CodexActivityOperationKey.AwaitingApproval),
            CodexActivityHookEvent.PostToolUse => (CodexActivityVisualState.Thinking, CodexActivityOperationKey.ReviewingToolResult),
            CodexActivityHookEvent.PreCompact => (CodexActivityVisualState.CompactingContext, CodexActivityOperationKey.CompactingContext),
            CodexActivityHookEvent.PostCompact => (CodexActivityVisualState.Thinking, CodexActivityOperationKey.ContinuingAfterCompaction),
            CodexActivityHookEvent.SubagentStart => (CodexActivityVisualState.Working, CodexActivityOperationKey.SubagentStarted),
            CodexActivityHookEvent.SubagentStop => (CodexActivityVisualState.Thinking, CodexActivityOperationKey.SubagentStopped),
            CodexActivityHookEvent.Stop => (CodexActivityVisualState.Completed, CodexActivityOperationKey.TurnCompleted),
            _ => (CodexActivityVisualState.Standby, CodexActivityOperationKey.ConnectingSession),
        };

        return new CodexActivitySnapshot(
            event.SessionHash,
            state,
            event.WorkspaceName,
            operation,
            event.ToolCategory,
            event.OccurredAt);
    }

    public static bool ShouldHideImmediately(CodexActivityEvent? event)
    {
        return event?.Event == CodexActivityHookEvent.SessionEnd;
    }

    public static bool ShouldStartInactivityCycle(CodexActivityEvent? event)
    {
        return event?.Event switch
        {
            CodexActivityHookEvent.Stop => true,
            CodexActivityHookEvent.SessionStart => event.SessionStartSource != CodexActivitySessionStartSource.Compact,
            _ => false,
        };
    }

    private static CodexActivityOperationKey OperationForTool(CodexActivityToolCategory? category)
    {
        return category switch
        {
            CodexActivityToolCategory.Shell => CodexActivityOperationKey.ExecutingShell,
            CodexActivityToolCategory.FileEdit => CodexActivityOperationKey.EditingFiles,
            CodexActivityToolCategory.Mcp => CodexActivityOperationKey.CallingExternalTool,
            CodexActivityToolCategory.Subagent => CodexActivityOperationKey.CoordinatingSubagent,
            CodexActivityToolCategory.LocalTool => CodexActivityOperationKey.UsingLocalTool,
            _ => CodexActivityOperationKey.UsingTool,
        };
    }
}
