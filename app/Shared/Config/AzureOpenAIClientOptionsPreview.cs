
using Azure.AI.OpenAI;

namespace Shared.Config;

public class AzureOpenAIClientOptionsPreview : AzureOpenAIClientOptions
{
    //
    // Summary:
    //     The version of the service to use.
    public enum ServiceVersionPreview
    {
        //
        // Summary:
        //     Service version "2024-04-01-preview".
        V2024_04_01_Preview = 7,
        V2024_04_15_Preview,
        V2024_05_01_Preview,
        V2024_06_01
    }

    private readonly string _versionPreview;


    private const ServiceVersionPreview LatestVersion = ServiceVersionPreview.V2024_05_01_Preview;

    internal string Version => _versionPreview;

    //
    // Summary:
    //     Initializes a new instance of Azure.AI.OpenAI.AzureOpenAIClientOptions
    //
    // Parameters:
    //   version:
    //     The service API version to use with the client.
    //
    // Exceptions:
    //   T:System.NotSupportedException:
    //     The provided service API version is not supported.
    public AzureOpenAIClientOptionsPreview(string version)
    {
        _versionPreview = version;
    }
}
