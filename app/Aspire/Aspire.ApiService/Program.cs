using Shared.Models;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.MapGet("/jobs", () =>
{
    var jobs = Enumerable.Range(1, 5).Select(index =>
        new BatchJob
        (
            Guid.NewGuid().ToString(),
            $"Job {index}",
            index % 2 == 0 ? "Succeeded" : "Failed",
            DateTimeOffset.Now.AddDays(-index),
            DateTimeOffset.Now.AddDays(-index).AddMinutes(5),
            DateTimeOffset.Now.AddDays(-index).AddMinutes(10),
            index % 2 == 0 ? null : "An error occurred."
        ))
        .ToArray();
    return jobs;
});

app.MapDefaultEndpoints();

app.Run();
