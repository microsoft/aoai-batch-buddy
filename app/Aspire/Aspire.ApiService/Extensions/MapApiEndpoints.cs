using Microsoft.AspNetCore.Mvc;
using OpenAI.Batch;
using OpenAI.Files;

namespace Aspire.ApiService.Extensions;

internal static class MapApiEndpoints
{
    internal static WebApplication MapApis(this WebApplication app)
    {
        app.MapGet("/jobs", OnGetBatchJobsAsync);

        app.MapGet("/files", OnGetFilesAsync);

        return app;
    }

    private static async Task<IResult> OnGetBatchJobsAsync(
        [FromServices] BatchClient client)
    {
        var jobs = await client.GetBatchesAsync("", 10, new());
        return TypedResults.Ok(jobs);
    }

    private static async Task<IResult> OnGetFilesAsync(
        [FromServices] FileClient client)
    {
        var files = await client.GetFilesAsync(OpenAIFilePurpose.Batch);
        return TypedResults.Ok(files.Value.ToArray());
    }
}
