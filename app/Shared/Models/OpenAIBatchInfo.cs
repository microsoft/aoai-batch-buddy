using System.ClientModel.Primitives;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.Models;

public partial class OpenAIBatchInfo
{
    public string Id { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public OpenAIBatchErrorCollection Errors { get; set; } = new([]);
    [JsonPropertyName("input_file_id")]
    public string InputFileId { get; set; } = string.Empty;
    [JsonPropertyName("completion_window")]
    public string CompletionWindow { get; set; } = "24h";
    public string Status { get; set; } = string.Empty;
    [JsonPropertyName("output_file_id")]
    public string OutputFileId { get; set; } = string.Empty;
    [JsonPropertyName("error_file_id")]
    public string ErrorFileId { get; set; } = string.Empty;
    [JsonPropertyName("created_at")]
    public long CreatedAt { get; set; }
    [JsonPropertyName("in_progress_at")]
    public long InProgressAt { get; set; }
    [JsonPropertyName("expires_at")]
    public long ExpiresAt { get; set; }
    [JsonPropertyName("finalizing_at")]
    public long FinalizingAt { get; set; }
    [JsonPropertyName("completed_at")]
    public long CompletedAt { get; set; }
    [JsonPropertyName("failed_at")]
    public long? FailedAt { get; set; }
    [JsonPropertyName("expired_at")]
    public long? ExpiredAt { get; set; }
    [JsonPropertyName("cancelling_at")]
    public long? CancellingAt { get; set; }
    [JsonPropertyName("cancelled_at")]
    public long? CancelledAt { get; set; }
    [JsonPropertyName("request_counts")]
    public BatchRequestCounts RequestCounts { get; set; } = new(0,0,0);
    public Dictionary<string,string>? Metadata { get; set; }

}

public record BatchRequestCounts(int Total, int Completed, int Failed);

public record OpenAIBatchError(string Code, string Message, string? Param = null, int? Line = null);

public record OpenAIBatchErrorCollection(IEnumerable<OpenAIBatchError> Data);

public readonly struct OpenAIBatchStatus : IEquatable<OpenAIBatchStatus>
{
    private readonly string _value;

    public static OpenAIBatchStatus Validating { get; } = new("validating");
    public static OpenAIBatchStatus Failed { get; } = new("failed");
    public static OpenAIBatchStatus InProgress { get; } = new("in_progress");
    public static OpenAIBatchStatus Finalizing { get; } = new("finalizing");
    public static OpenAIBatchStatus Completed { get; } = new("completed");
    public static OpenAIBatchStatus Expired { get; } = new("expired");
    public static OpenAIBatchStatus Cancelling { get; } = new("cancelling");
    public static OpenAIBatchStatus Cancelled { get; } = new("cancelled");

    public OpenAIBatchStatus(string value)
    {
        _value = value ?? throw new ArgumentNullException("value");
    }

    public static bool operator ==(OpenAIBatchStatus left, OpenAIBatchStatus right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(OpenAIBatchStatus left, OpenAIBatchStatus right)
    {
        return !left.Equals(right);
    }

    public static implicit operator OpenAIBatchStatus(string value)
    {
        return new OpenAIBatchStatus(value);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj)
    {
        if (obj is OpenAIBatchStatus other)
        {
            return Equals(other);
        }

        return false;
    }

    public bool Equals(OpenAIBatchStatus other)
    {
        return string.Equals(_value, other._value, StringComparison.InvariantCultureIgnoreCase);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode()
    {
        return _value?.GetHashCode() ?? 0;
    }

    public override string ToString()
    {
        return _value;
    }
}
