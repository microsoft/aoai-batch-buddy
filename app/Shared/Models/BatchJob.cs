
namespace Shared.Models;

public record BatchJob(string Id, string Name, string Status, DateTimeOffset CreatedAt, DateTimeOffset? StartedAt, DateTimeOffset? CompletedAt, string? Error)
{
    public string StatusText => Status switch
    {
        "Succeeded" => "success",
        "Failed" => "danger",
        "Running" => "info",
        _ => "warning"
    };
}
