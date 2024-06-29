using OpenAI;
using OpenAI.Batch;
using OpenAI.Files;

namespace Aspire.ApiService.Extensions;

internal static class OpenAIServices
{
    internal static IServiceCollection AddOpenAIServices(this IServiceCollection services)
    {
        // Register BatchClient
        services.AddSingleton(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var options = new OpenAIClientOptions
            {
                Endpoint = new Uri(config["OPENAI_ENDPOINT"] ?? "")
            };
            return new BatchClient(options);
        });

        // Register FileClient
        services.AddSingleton(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var options = new OpenAIClientOptions
            {
                Endpoint = new Uri(config["OPENAI_ENDPOINT"] ?? "")                
            };
            return new FileClient(options);
        });

        return services;
    }
}
