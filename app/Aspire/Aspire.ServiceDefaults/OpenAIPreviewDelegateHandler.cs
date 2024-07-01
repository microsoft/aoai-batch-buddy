using System.Web;

namespace Aspire.ServiceDefaults;

public class OpenAIPreviewDelegateHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.RequestUri == null)
        {
            return base.SendAsync(request, cancellationToken);
        }

        var qs = HttpUtility.ParseQueryString(request.RequestUri?.Query ?? "");
        if (qs.AllKeys.Contains("api-version"))
        {
            qs.Set("api-version", "2024-04-15-preview");
            request.RequestUri = new Uri(request.RequestUri?.GetLeftPart(UriPartial.Path) + "?" + qs.ToString());
        }

        // Continue processing
        return base.SendAsync(request, cancellationToken);
    }
}
