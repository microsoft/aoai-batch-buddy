
using System.Text.Json.Serialization;

namespace Shared.Models;

public class BatchJob
{
    public string Id { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    [JsonPropertyName("input_file_id")]
    public string InputFileId { get; set; } = string.Empty;
    [JsonPropertyName("completion_window")]
    public string CompletionWindow { get; set; } = string.Empty;
    public BatchStatus Status { get; set; }
    [JsonPropertyName("output_file_id")]
    public string OutputFileId { get; set; } = string.Empty;
    [JsonPropertyName("error_file_id")]
    public string ErrorFileId { get; set; } = string.Empty;
    [JsonPropertyName("created_at")]
    public long CreatedAt { get; set; }
    [JsonPropertyName("in_progress_at")]
    public long? InProgressAt { get; set; }
    [JsonPropertyName("expires_at")]
    public long ExpiresAt { get; set; }
    [JsonPropertyName("completed_at")]
    public long? CompletedAt { get; set; }
    [JsonPropertyName("failed_at")]
    public long? FailedAt { get; set; }
    [JsonPropertyName("expired_at")]
    public long? ExpiredAt { get; set; }
    [JsonPropertyName("request_counts")]
    public BatchRequestCounts RequestCounts { get; set; } = new(0,0,0);
    public Dictionary<string,string>? Metadata { get; set; }
    //        {
    //  "object": "batch",
    //  "errors": null,
    //}
}

public record BatchRequestCounts(int Total, int Completed, int Failed);

public enum BatchStatus
{
    Validating,     // the input file is being validated before the batch can begin
    Failed,         // the input file has failed the validation process
    InProgress,     // the input file was successfully validated and the batch is currently being run
    Finalizing,     // the batch has completed and the results are being prepared
    Completed,      // the batch has been completed and the results are ready
    Expired,        // the batch was not able to be completed within the 24-hour time window
    Cancelling,     // the batch is being cancelled (may take up to 10 minutes)
    Cancelled      // the batch was cancelled
} 
