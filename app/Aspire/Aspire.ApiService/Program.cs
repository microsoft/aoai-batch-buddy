using Aspire.ApiService.Extensions;
using System.Web;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add OpenAI Services
builder.Services.AddOpenAIServices();

// Add services to the container.
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

// Map endpoints found in ./Extensions/MapApiEndpoints.cs
app.MapApis();

app.Use(async (context, next) =>
{
    var qs = HttpUtility.ParseQueryString(context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty);
    if (qs.AllKeys.Contains("api-version"))
    {
        qs.Set("api-version", "2024-04-15-preview");
        context.Request.QueryString = new QueryString(qs.ToString());
    }

    // Call the next delegate/middleware in the pipeline.
    await next(context);
});

app.MapDefaultEndpoints();

app.Run();
