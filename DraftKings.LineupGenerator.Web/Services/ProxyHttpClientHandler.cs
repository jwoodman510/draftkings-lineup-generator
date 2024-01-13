
using System.Web;

namespace DraftKings.LineupGenerator.Web.Services
{
    public class ProxyHttpClientHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri != null)
            {
                var originalUrl = request.RequestUri.ToString();
                var encodedUrl = HttpUtility.UrlEncode(originalUrl);

                request.RequestUri = new Uri($"https://corsproxy.io/?{encodedUrl}");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
