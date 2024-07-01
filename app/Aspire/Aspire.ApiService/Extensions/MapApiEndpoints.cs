using Microsoft.AspNetCore.Mvc;
using OpenAI.Batch;
using OpenAI.Files;
using Shared.Models;
using System.ClientModel.Primitives;

namespace Aspire.ApiService.Extensions;

internal static class MapApiEndpoints
{
    internal static WebApplication MapApis(this WebApplication app)
    {
        // GET /jobs - Get list of batch jobs
        app.MapGet("/jobs", OnGetBatchJobsAsync);

        // GET /jobs/{id} - Get batch job by id
        //app.MapGet("/jobs/{id}", OnGetBatchJobByIdAsync);

        //// POST /jobs - Add a new batch job
        //app.MapPost("/jobs", OnPostBatchJobAsync);

        //// DELETE /jobs/{id} - Delete batch job by id
        //app.MapDelete("/jobs/{id}", OnDeleteBatchJobByIdAsync);

        // GET /files - Get list of files
        app.MapGet("/files", OnGetFilesAsync);

        //// GET /files/{id} - Get file by id
        //app.MapGet("/files/{id}", OnGetFileByIdAsync);

        //// POST /files - Add a new file
        //app.MapPost("/files", OnPostFileAsync);

        //// DELETE /files/{id} - Delete file by id
        //app.MapDelete("/files/{id}", OnDeleteFileByIdAsync);

        return app;
    }

   
    
    // BATCH JOBS
    private static async Task<IResult> OnGetBatchJobsAsync(
        [FromServices] BatchClient client,
        CancellationToken cancellationToken)
    {
        var requestOptions = new RequestOptions { CancellationToken = cancellationToken };
        requestOptions.AddPolicy(new OpenAIBatchPreviewPolicy(), 0);
        var response = await client.GetBatchesAsync("", 10, requestOptions);
        var jobs = new OpenAIBatchInfo();
        jobs.Create(response.GetRawResponse().Content, new("J"));
        return TypedResults.Ok(jobs);
    }

    private static async Task<IResult> OnGetBatchJobByIdAsync(
        [FromServices] BatchClient client,
        [FromRoute] string id,
        CancellationToken cancellationToken)
    {
        var job = await client.GetBatchAsync(id, new() { CancellationToken = cancellationToken });
        return TypedResults.Ok(job);
    }

    private static async Task<IResult> OnPostBatchJobAsync(
        [FromServices] BatchClient client,
        OpenAIBatchCreateRequest batch,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
        // var job = await client.CreateBatchAsync(batch, new() { CancellationToken = cancellationToken });
        // return Results.Created($"/jobs/{job.Id}", job);
    }

    private static async Task<IResult> OnDeleteBatchJobByIdAsync(
        [FromRoute] string id,
        [FromServices] BatchClient client,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }


    // FILES
    private static async Task<IResult> OnGetFilesAsync(
        [FromServices] FileClient client)
    {
        try
        {
            var files = await client.GetFilesAsync(OpenAIFilePurpose.Batch);
            return TypedResults.Ok(files.Value.ToArray());
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex);
        }
    }
        

    private static async Task<IResult> OnGetFileByIdAsync(
        [FromRoute] string id,
        [FromServices] FileClient client,
        CancellationToken cancellationToken)
    {
        //VNEXT var file = await client.GetFileAsync(id, cancellationToken);
        var file = await client.GetFileAsync(id);
        return TypedResults.Ok(file.Value);
    }

    private static async Task<IResult> OnPostFileAsync(
        [FromForm] IFormFile file,
        [FromServices] FileClient client,
        CancellationToken cancellationToken)
    {
        var fs = new MemoryStream();
        await file.CopyToAsync(fs);
        var fileInfo = await client.UploadFileAsync(fs, file.FileName, FileUploadPurpose.Batch);
        return Results.Created($"/files/{fileInfo.Value.Id}", fileInfo);
    }

    private static async Task<IResult> OnDeleteFileByIdAsync(
        [FromRoute] string id,
        [FromServices] FileClient client,
        CancellationToken cancellationToken)
    {
        var file = await client.GetFileAsync(id);
        if (file.Value != null)
        {
            await client.DeleteFileAsync(file.Value);
            return Results.NoContent();
        }
        else
        {
            return Results.NotFound();
        }

    }

}
