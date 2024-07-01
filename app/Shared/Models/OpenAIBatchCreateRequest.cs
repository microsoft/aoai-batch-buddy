using System.Text.Json.Serialization;

namespace Shared.Models;

public class OpenAIBatchCreateRequest
{
    [JsonPropertyName("input_file_id")]
    public string InputFileId { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    [JsonPropertyName("completion_window")]
    public string CompletionWindow { get; set; } = string.Empty;
    public Dictionary<string,string>? Metadata { get; set; }
}
