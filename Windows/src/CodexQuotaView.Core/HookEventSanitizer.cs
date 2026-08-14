using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CodexQuotaView.Core;

public static class HookEventSanitizer
{
    private static readonly HashSet<string> AllowedKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "schema_version",
        "event",
        "session_hash",
        "turn_hash",
        "workspace_name",
        "tool_category",
        "session_start_source",
        "occurred_at",
    };

    private static readonly HashSet<string> SensitiveKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "prompt",
        "command",
        "arguments",
        "args",
        "output",
        "result",
        "cwd",
        "path",
        "file",
        "session_id",
        "turn_id",
        "token",
        "cookie",
        "auth",
        "authorization",
        "account",
        "api_key",
    };

    public static string HashIdentifier(string identifier)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(identifier));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public static string? SanitizeWorkspaceName(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }
        var name = Path.GetFileName(path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        return string.IsNullOrWhiteSpace(name) ? null : name[..Math.Min(name.Length, 80)];
    }

    public static bool IsSensitiveKey(string key)
    {
        return SensitiveKeys.Contains(key);
    }

    public static JsonDocument? SanitizePayload(string json, out string? rejectionReason)
    {
        rejectionReason = null;
        try
        {
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
            {
                rejectionReason = "payload_must_be_object";
                return null;
            }

            foreach (var property in root.EnumerateObject())
            {
                var key = property.Name;
                if (SensitiveKeys.Contains(key))
                {
                    rejectionReason = "sensitive_field_" + key.ToLowerInvariant();
                    return null;
                }
                if (property.Value.ValueKind == JsonValueKind.Object)
                {
                    foreach (var nested in property.Value.EnumerateObject())
                    {
                        if (SensitiveKeys.Contains(nested.Name))
                        {
                            rejectionReason = "sensitive_nested_field_" + nested.Name.ToLowerInvariant();
                            return null;
                        }
                    }
                }
            }

            var filtered = new Dictionary<string, JsonElement>();
            foreach (var property in root.EnumerateObject())
            {
                if (AllowedKeys.Contains(property.Name))
                {
                    filtered[property.Name] = property.Value.Clone();
                }
            }
            var bytes = JsonSerializer.SerializeToUtf8Bytes(filtered);
            return JsonDocument.Parse(bytes);
        }
        catch (JsonException)
        {
            rejectionReason = "invalid_json";
            return null;
        }
    }
}
