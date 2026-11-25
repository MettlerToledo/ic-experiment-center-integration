// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System.Diagnostics;
using System.Net;
using MettlerToledo.Labs.Experiments.Clients.Experiments;
using MettlerToledo.Labs.Experiments.Clients.Experiments.PublicApi;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MettlerToledo.Labs.Experiments.iCDataCenterConverter.Exporting;

/// <summary>
/// Exports a report for each experiment that becomes concluded on the public experiments API state change stream.
/// </summary>
public sealed class ConcludedExperimentExporter : IConcludedExperimentExporter
{
    private readonly IExperimentsPublicApiClient _client;
    private readonly IReportFileWriter _fileWriter;
    private readonly ConcludedExperimentExportOptions _options;
    private readonly ILogger<ConcludedExperimentExporter> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="ConcludedExperimentExporter"/>.
    /// </summary>
    public ConcludedExperimentExporter(
        IExperimentsPublicApiClient client,
        IReportFileWriter fileWriter,
        IOptions<ConcludedExperimentExportOptions> options,
        ILogger<ConcludedExperimentExporter> logger)
    {
        _client = client;
        _fileWriter = fileWriter;
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task Run(ReportFileType fileType, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Watching for concluded experiments. Exporting {FileType}.", fileType);

        var reconnectDelay = _options.InitialReconnectDelay;
        while (!cancellationToken.IsCancellationRequested)
        {
            var connectedAt = Stopwatch.GetTimestamp();
            try
            {
                await ExportConcludedExperiments(fileType, cancellationToken);
                _logger.LogWarning("The experiment state change stream was closed by the server.");
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (HttpRequestException exception) when (IsAccessDenied(exception))
            {
                _logger.LogCritical("The server rejected the API key ({StatusCode}). Stopping.", exception.StatusCode);
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogWarning("Lost connection to the experiment state change stream: {Error}", exception.Message);
            }

            if (Stopwatch.GetElapsedTime(connectedAt) >= _options.MaxReconnectDelay)
            {
                reconnectDelay = _options.InitialReconnectDelay;
            }

            _logger.LogInformation("Reconnecting in {ReconnectDelay}.", reconnectDelay);
            try
            {
                await Task.Delay(reconnectDelay, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            reconnectDelay = reconnectDelay * 2 > _options.MaxReconnectDelay ? _options.MaxReconnectDelay : reconnectDelay * 2;
        }

        _logger.LogInformation("Stopped watching for concluded experiments.");
    }

    private static bool IsAccessDenied(HttpRequestException exception) =>
        exception.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden;

    private async Task ExportConcludedExperiments(ReportFileType fileType, CancellationToken cancellationToken)
    {
        await foreach (var stateChange in _client.StreamStateChanges(cancellationToken))
        {
            if (stateChange.State == ExperimentStateJson.Concluded)
            {
                await ExportReport(stateChange.ExperimentId, fileType, cancellationToken);
            }
        }
    }

    private async Task ExportReport(Guid experimentId, ReportFileType fileType, CancellationToken cancellationToken)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(_options.ReportExportTimeout);

        try
        {
            using var report = await _client.DownloadReport(experimentId, fileType, timeout.Token);
            var filePath = await _fileWriter.Write(report, timeout.Token);

            _logger.LogInformation("Exported concluded experiment {ExperimentId} to {FilePath}.", experimentId, filePath);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (OperationCanceledException) when (timeout.IsCancellationRequested)
        {
            _logger.LogError(
                "Timed out after {Timeout} exporting concluded experiment {ExperimentId}.",
                _options.ReportExportTimeout,
                experimentId);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to export concluded experiment {ExperimentId}.", experimentId);
        }
    }
}
