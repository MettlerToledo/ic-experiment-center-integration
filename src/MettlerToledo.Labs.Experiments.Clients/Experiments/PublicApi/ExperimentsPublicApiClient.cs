// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.PublicApi;

/// <summary>
/// A client for the iC Experiment Center public experiments API.
/// </summary>
public sealed class ExperimentsPublicApiClient : IExperimentsPublicApiClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of <see cref="ExperimentsPublicApiClient"/>.
    /// </summary>
    public ExperimentsPublicApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<ExperimentStateChangeJson> StreamStateChanges(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, PublicApiUrlConstants.ExperimentStateChangesPath);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(PublicApiUrlConstants.EventStreamMediaType));

        using var response = await _httpClient
            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        var parser = SseParser.Create(stream, static (_, data) =>
            JsonSerializer.Deserialize<ExperimentStateChangeJson>(data, ExperimentJsonSerializer.SerializerOptions));

        await foreach (var item in parser.EnumerateAsync(cancellationToken))
        {
            if (item.Data is not null)
            {
                yield return item.Data;
            }
        }
    }

    /// <inheritdoc />
    public async Task<DownloadedReport> DownloadReport(
        Guid experimentId,
        ReportFileType format,
        CancellationToken cancellationToken = default)
    {
        var requestUri =
            $"{PublicApiUrlConstants.PublicExperimentsPath}/{experimentId}/{PublicApiUrlConstants.ReportName}" +
            $"?{PublicApiUrlConstants.FormatQueryParameterName}={format}";

        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

        var response = await _httpClient
            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        try
        {
            response.EnsureSuccessStatusCode();

            var fileName = GetFileName(response, experimentId, format);
            var content = await response.Content.ReadAsStreamAsync();
            return new DownloadedReport(fileName, content, response);
        }
        catch
        {
            response.Dispose();
            throw;
        }
    }

    private static string GetFileName(HttpResponseMessage response, Guid experimentId, ReportFileType format)
    {
        var contentDisposition = response.Content.Headers.ContentDisposition;
        var serverFileName = contentDisposition?.FileNameStar ?? contentDisposition?.FileName;

        if (serverFileName is not null && !string.IsNullOrWhiteSpace(serverFileName))
        {
            return serverFileName.Trim('"');
        }

        return $"{experimentId}{format.GetFileExtension()}";
    }
}
