using Azure.Core;
using System.ClientModel.Primitives;
using System.Runtime.ExceptionServices;
using System.Web;

namespace Aspire.ApiService.Extensions;

public class OpenAIBatchPreviewPolicy : PipelinePolicy
{
    public override void Process(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int currentIndex)
    {
        var previewUri = GetApiPreviewUri(message.Request.Uri);
        message.Request.Uri = previewUri;

        ProcessNext(message, pipeline, currentIndex);
    }

    public override async ValueTask ProcessAsync(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int currentIndex)
    {
        var previewUri = GetApiPreviewUri(message.Request.Uri);
        message.Request.Uri = previewUri;

        await ProcessNextAsync(message, pipeline, currentIndex);
    }
    

    private static Uri GetApiPreviewUri(Uri endpoint)
    {
        var qs = HttpUtility.ParseQueryString(endpoint.Query ?? "");
        if (qs.AllKeys.Contains("api-version"))
        {
            qs.Set("api-version", "2024-04-15-preview");
            return new Uri(endpoint?.GetLeftPart(UriPartial.Path) + "?" + qs.ToString());
        }

        return endpoint;
    }

}
