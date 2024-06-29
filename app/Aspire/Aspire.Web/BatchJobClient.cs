using Shared.Models;

namespace Aspire.Web;

public class BatchJobClient
{
    private readonly HttpClient _httpClient;

    public BatchJobClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<BatchJob[]> GetBatchJobsAsync(int maxItems = 10, CancellationToken cancellationToken = default)
    {
        List<BatchJob>? jobs = null;

        await foreach (var job in _httpClient.GetFromJsonAsAsyncEnumerable<BatchJob>("/jobs", cancellationToken))
        {
            if (jobs?.Count >= maxItems)
            {
                break;
            }
            if (job is not null)
            {
                jobs ??= [];
                jobs.Add(job);
            }
        }

        return jobs?.ToArray() ?? [];
    }
}
