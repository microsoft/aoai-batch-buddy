using OpenAI.Files;
using Shared.Models;

namespace Aspire.Web;

public class ApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<OpenAIBatchInfo[]> GetBatchJobsAsync(int maxItems = 10, CancellationToken cancellationToken = default)
    {
        var results = await _httpClient.GetFromJsonAsync<OpenAIBatchInfo[]>("/jobs", cancellationToken);
        return results ?? [];
    }

    public async Task<OpenAIFileInfo[]> GetFilesAsync(CancellationToken cancellationToken = default)
    {
        var results = await _httpClient.GetFromJsonAsync<OpenAIFileInfo[]>("/files", cancellationToken);
        return results ?? [];
    }
}
