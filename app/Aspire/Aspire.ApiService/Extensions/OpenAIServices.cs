using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using Shared.Config;
using System.ClientModel;

namespace Aspire.ApiService.Extensions;

internal static class OpenAIServices
{
    internal static IServiceCollection AddOpenAIServices(this IServiceCollection services)
    {

        // Register Azure OpenAI client
        services.AddSingleton<OpenAIClient>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var useAoai = bool.Parse(config["USE_AOAI"] ?? "false");
            if (useAoai)
            {
                var endpoint = new Uri(config["AZURE_OPENAI_ENDPOINT"] ?? "");
                var keyCreds = new AzureKeyCredential(config["AZURE_OPENAI_API_KEY"] ?? "");
                var options = new AzureOpenAIClientOptionsPreview("2024-04-15-preview");                
                var client = new AzureOpenAIClient(endpoint, keyCreds, options);
                return client;
            }
            else
            {
                var endpoint = new Uri(config["OPENAI_ENDPOINT"] ?? "");
                var options = new OpenAIClientOptions { Endpoint = endpoint };
                var key = new ApiKeyCredential(config["OPENAI_API_KEY"] ?? "");
                
                return new OpenAIClient(key);
            }
        });

        // Register BatchClient
        services.AddSingleton(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var client = sp.GetRequiredService<OpenAIClient>();
            if (bool.Parse(config["USE_AOAI"] ?? "false"))
            {
                return ((AzureOpenAIClient)client).GetBatchClient(config["AZURE_OPENAI_DEPLOYMENT_NAME"]);
            }
            else
            {
                return client.GetBatchClient();
            }
        });

        // Register FileClient
        services.AddSingleton(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var client = sp.GetRequiredService<OpenAIClient>();
            return client.GetFileClient();
        });

        return services;
    }
}
