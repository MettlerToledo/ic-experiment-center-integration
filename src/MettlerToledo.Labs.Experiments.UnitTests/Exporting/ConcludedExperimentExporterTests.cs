// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System.Net;
using System.Runtime.ExceptionServices;
using MettlerToledo.Labs.Experiments.Clients.Experiments;
using MettlerToledo.Labs.Experiments.Clients.Experiments.PublicApi;
using MettlerToledo.Labs.Experiments.iCDataCenterConverter.Exporting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using Xunit;

namespace MettlerToledo.Labs.Experiments.UnitTests.Exporting;

public class ConcludedExperimentExporterTests : IDisposable
{
    private static readonly Guid FirstExperimentId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid SecondExperimentId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    private readonly IExperimentsPublicApiClient _client = Substitute.For<IExperimentsPublicApiClient>();
    private readonly IReportFileWriter _fileWriter = Substitute.For<IReportFileWriter>();
    private readonly CancellationTokenSource _cancellation =
        CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
    private readonly ConcludedExperimentExportOptions _options = new()
    {
        InitialReconnectDelay = TimeSpan.Zero,
        MaxReconnectDelay = TimeSpan.Zero,
    };

    [Fact]
    public async Task RunAsync_ConcludedExperiment_DownloadsAndWritesReport()
    {
        var report = NewReport();
        _client.StreamStateChangesAsync(Arg.Any<CancellationToken>())
            .Returns(StateChanges(StateChange(FirstExperimentId, ExperimentStateJson.Concluded)), StopWatching());
        _client.DownloadReportAsync(FirstExperimentId, ReportFileType.Pdf, Arg.Any<CancellationToken>())
            .Returns(report);

        await RunExporterAsync();

        await _fileWriter.Received(1).WriteAsync(report, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RunAsync_NonConcludedEvents_DoesNotExport()
    {
        _client.StreamStateChangesAsync(Arg.Any<CancellationToken>())
            .Returns(
                StateChanges(
                    StateChange(FirstExperimentId, ExperimentStateJson.Planning),
                    StateChange(FirstExperimentId, ExperimentStateJson.InProgress)),
                StopWatching());

        await RunExporterAsync();

        await _client.DidNotReceive().DownloadReportAsync(Arg.Any<Guid>(), Arg.Any<ReportFileType>(), Arg.Any<CancellationToken>());
        await _fileWriter.DidNotReceive().WriteAsync(Arg.Any<DownloadedReport>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RunAsync_DownloadFails_LogsAndContinuesWithNextConcludedExperiment()
    {
        var secondReport = NewReport();
        _client.StreamStateChangesAsync(Arg.Any<CancellationToken>())
            .Returns(ConcludedFirstAndSecond(), StopWatching());
        _client.DownloadReportAsync(FirstExperimentId, ReportFileType.Pdf, Arg.Any<CancellationToken>())
            .Throws(new HttpRequestException("boom"));
        _client.DownloadReportAsync(SecondExperimentId, ReportFileType.Pdf, Arg.Any<CancellationToken>())
            .Returns(secondReport);

        await RunExporterAsync();

        await _fileWriter.Received(1).WriteAsync(Arg.Any<DownloadedReport>(), Arg.Any<CancellationToken>());
        await _fileWriter.Received(1).WriteAsync(secondReport, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RunAsync_ReportExportTimesOut_LogsAndContinuesWithNextConcludedExperiment()
    {
        _options.ReportExportTimeout = TimeSpan.FromMilliseconds(50);
        var secondReport = NewReport();
        _client.StreamStateChangesAsync(Arg.Any<CancellationToken>())
            .Returns(ConcludedFirstAndSecond(), StopWatching());
        _client.DownloadReportAsync(FirstExperimentId, ReportFileType.Pdf, Arg.Any<CancellationToken>())
            .Returns(callInfo => NeverCompletesAsync(callInfo.ArgAt<CancellationToken>(2)));
        _client.DownloadReportAsync(SecondExperimentId, ReportFileType.Pdf, Arg.Any<CancellationToken>())
            .Returns(secondReport);

        await RunExporterAsync();

        await _fileWriter.Received(1).WriteAsync(Arg.Any<DownloadedReport>(), Arg.Any<CancellationToken>());
        await _fileWriter.Received(1).WriteAsync(secondReport, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RunAsync_StreamClosedByServer_ReconnectsAndKeepsExporting()
    {
        var report = NewReport();
        _client.StreamStateChangesAsync(Arg.Any<CancellationToken>())
            .Returns(
                StateChanges(),
                StateChanges(StateChange(FirstExperimentId, ExperimentStateJson.Concluded)),
                StopWatching());
        _client.DownloadReportAsync(FirstExperimentId, ReportFileType.Pdf, Arg.Any<CancellationToken>())
            .Returns(report);

        await RunExporterAsync();

        _client.Received(3).StreamStateChangesAsync(Arg.Any<CancellationToken>());
        await _fileWriter.Received(1).WriteAsync(report, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RunAsync_StreamFails_ReconnectsAndKeepsExporting()
    {
        var report = NewReport();
        _client.StreamStateChangesAsync(Arg.Any<CancellationToken>())
            .Returns(
                Failing(new HttpRequestException("Connection refused.")),
                StateChanges(StateChange(FirstExperimentId, ExperimentStateJson.Concluded)),
                StopWatching());
        _client.DownloadReportAsync(FirstExperimentId, ReportFileType.Pdf, Arg.Any<CancellationToken>())
            .Returns(report);

        await RunExporterAsync();

        _client.Received(3).StreamStateChangesAsync(Arg.Any<CancellationToken>());
        await _fileWriter.Received(1).WriteAsync(report, Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden)]
    public async Task RunAsync_ApiKeyRejected_ThrowsWithoutReconnecting(HttpStatusCode statusCode)
    {
        _client.StreamStateChangesAsync(Arg.Any<CancellationToken>())
            .Returns(Failing(new HttpRequestException("Rejected.", null, statusCode)));

        var exception = await Should.ThrowAsync<HttpRequestException>(RunExporterAsync);

        exception.StatusCode.ShouldBe(statusCode);
        _client.Received(1).StreamStateChangesAsync(Arg.Any<CancellationToken>());
    }

    public void Dispose()
    {
        _cancellation.Dispose();
    }

    private Task RunExporterAsync() =>
        new ConcludedExperimentExporter(
                _client,
                _fileWriter,
                Options.Create(_options),
                NullLogger<ConcludedExperimentExporter>.Instance)
            .RunAsync(ReportFileType.Pdf, _cancellation.Token);

    private async IAsyncEnumerable<ExperimentStateChangeJson> StopWatching()
    {
        await _cancellation.CancelAsync();
        _cancellation.Token.ThrowIfCancellationRequested();
        yield break;
    }

    private static DownloadedReport NewReport() =>
        new("experiment.pdf", new MemoryStream(new byte[] { 1, 2, 3 }));

    private static ExperimentStateChangeJson StateChange(Guid experimentId, ExperimentStateJson state) =>
        new() { ExperimentId = experimentId, State = state, Timestamp = DateTime.UtcNow };

    private static IAsyncEnumerable<ExperimentStateChangeJson> ConcludedFirstAndSecond() =>
        StateChanges(
            StateChange(FirstExperimentId, ExperimentStateJson.Concluded),
            StateChange(SecondExperimentId, ExperimentStateJson.Concluded));

    private static async IAsyncEnumerable<ExperimentStateChangeJson> StateChanges(params ExperimentStateChangeJson[] stateChanges)
    {
        await Task.CompletedTask;
        foreach (var stateChange in stateChanges)
        {
            yield return stateChange;
        }
    }

    private static async IAsyncEnumerable<ExperimentStateChangeJson> Failing(Exception exception)
    {
        await Task.Yield();
        ExceptionDispatchInfo.Throw(exception);
        yield break;
    }

    private static async Task<DownloadedReport> NeverCompletesAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
        throw new InvalidOperationException("The delay only ends by cancellation.");
    }
}
