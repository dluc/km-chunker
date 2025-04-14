// Copyright (c) Microsoft. All rights reserved.

using System.Net;
using Polly;

namespace Microsoft.KernelMemory.Chunker.Internals;

internal sealed class WebScraper : IDisposable
{
    private readonly HttpClient _httpClient;

    public WebScraper(HttpClient? httpClient = null)
    {
        this._httpClient = httpClient ?? new HttpClient();
    }

    public async Task<string> GetAsync(Uri url, CancellationToken cancellationToken = default)
    {
        if (url.Scheme.ToUpperInvariant() is not "HTTP" and not "HTTPS")
        {
            throw new WebScraperException($"Unknown URL protocol: {url.Scheme}");
        }

        HttpResponseMessage response = await RetryLogic()
            .ExecuteAsync(async _ => await this._httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw new WebScraperException($"HTTP error, status code: {response.StatusCode}");
        }

        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    }

    public void Dispose()
    {
        this._httpClient.Dispose();
    }

    private static ResiliencePipeline<HttpResponseMessage> RetryLogic()
    {
        var retriableErrors = new[]
        {
            HttpStatusCode.RequestTimeout, // 408
            HttpStatusCode.InternalServerError, // 500
            HttpStatusCode.BadGateway, // 502
            HttpStatusCode.GatewayTimeout, // 504
        };

        const int MaxDelay = 5;
        var delays = new List<int> { 1, 1, 1, 2, 2, 3, 4, MaxDelay };

        return new ResiliencePipelineBuilder<HttpResponseMessage>()
            .AddRetry(new()
            {
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .HandleResult(resp => retriableErrors.Contains(resp.StatusCode)),
                MaxRetryAttempts = 10,
                DelayGenerator = args =>
                {
                    double secs = (args.AttemptNumber < delays.Count) ? delays[args.AttemptNumber] : MaxDelay;
                    return ValueTask.FromResult<TimeSpan?>(TimeSpan.FromSeconds(secs));
                }
            })
            .Build();
    }
}
