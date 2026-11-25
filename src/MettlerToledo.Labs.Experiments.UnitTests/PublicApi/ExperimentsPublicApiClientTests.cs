// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System.Net;
using System.Net.Http.Headers;
using System.Text;
using MettlerToledo.Labs.Experiments.Clients.Experiments;
using MettlerToledo.Labs.Experiments.Clients.Experiments.PublicApi;
using Shouldly;
using Xunit;

namespace MettlerToledo.Labs.Experiments.UnitTests.PublicApi;

public class ExperimentsPublicApiClientTests
{
    private const string PdfMediaType = "application/pdf";
    private const string DocxMediaType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

    private static readonly Guid ExperimentId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [Fact]
    public async Task StreamStateChangesAsync_WithConcludedEvent_YieldsStateChange()
    {
        var timestamp = new DateTime(2026, 11, 24, 10, 0, 0, DateTimeKind.Utc);
        var body =
            $"data: {{\"experimentId\":\"{ExperimentId}\",\"state\":\"Concluded\",\"timestamp\":\"2026-11-24T10:00:00Z\"}}\n\n";
        var (client, _) = CreateClient(_ => EventStreamResponse(body));

        var stateChanges = await ReadAllAsync(client.StreamStateChangesAsync(TestContext.Current.CancellationToken));

        var stateChange = stateChanges.ShouldHaveSingleItem();
        stateChange.ExperimentId.ShouldBe(ExperimentId);
        stateChange.State.ShouldBe(ExperimentStateJson.Concluded);
        stateChange.Timestamp.ToUniversalTime().ShouldBe(timestamp);
    }

    [Fact]
    public async Task StreamStateChangesAsync_MultipleEvents_YieldsEachInOrder()
    {
        var body =
            $"data: {{\"experimentId\":\"{ExperimentId}\",\"state\":\"InProgress\",\"timestamp\":\"2026-11-24T10:00:00Z\"}}\n\n" +
            $"data: {{\"experimentId\":\"{ExperimentId}\",\"state\":\"Concluded\",\"timestamp\":\"2026-11-24T11:00:00Z\"}}\n\n";
        var (client, _) = CreateClient(_ => EventStreamResponse(body));

        var stateChanges = await ReadAllAsync(client.StreamStateChangesAsync(TestContext.Current.CancellationToken));

        stateChanges.Count.ShouldBe(2);
        stateChanges[0].State.ShouldBe(ExperimentStateJson.InProgress);
        stateChanges[1].State.ShouldBe(ExperimentStateJson.Concluded);
    }

    [Fact]
    public async Task StreamStateChangesAsync_NonSuccessStatus_Throws()
    {
        var (client, _) = CreateClient(_ => new HttpResponseMessage(HttpStatusCode.Unauthorized));

        await Should.ThrowAsync<HttpRequestException>(async () =>
            await ReadAllAsync(client.StreamStateChangesAsync(TestContext.Current.CancellationToken)));
    }

    [Fact]
    public async Task DownloadReportAsync_Pdf_RequestsReportUrlWithFormatAndReturnsServerFileName()
    {
        var reportBytes = Encoding.UTF8.GetBytes("%PDF-1.7 report");
        var (client, handler) = CreateClient(_ => ReportResponse(reportBytes, PdfMediaType, Attachment("My Experiment.pdf")));

        using var report = await client.DownloadReportAsync(ExperimentId, ReportFileType.Pdf, TestContext.Current.CancellationToken);

        var requestUri = handler.LastRequest.ShouldNotBeNull().RequestUri.ShouldNotBeNull();
        requestUri.AbsolutePath.ShouldBe($"/api/public/v1/experiments/{ExperimentId}/report");
        requestUri.Query.ShouldBe("?format=Pdf");
        report.FileName.ShouldBe("My Experiment.pdf");
        (await ReadAllBytesAsync(report.Content)).ShouldBe(reportBytes);
    }

    [Fact]
    public async Task DownloadReportAsync_Docx_UsesDocxFormat()
    {
        var (client, handler) = CreateClient(_ => ReportResponse([1, 2, 3], DocxMediaType, Attachment("My Experiment.docx")));

        using var report = await client.DownloadReportAsync(ExperimentId, ReportFileType.Docx, TestContext.Current.CancellationToken);

        handler.LastRequest.ShouldNotBeNull().RequestUri.ShouldNotBeNull().Query.ShouldBe("?format=Docx");
        report.FileName.ShouldBe("My Experiment.docx");
    }

    [Fact]
    public async Task DownloadReportAsync_NonAsciiFileName_ReturnsDecodedFileName()
    {
        var contentDisposition = new ContentDispositionHeaderValue("attachment")
        {
            FileName = "Versuch _.pdf",
            FileNameStar = "Versuch Ä.pdf",
        };
        var (client, _) = CreateClient(_ => ReportResponse([1], PdfMediaType, contentDisposition));

        using var report = await client.DownloadReportAsync(ExperimentId, ReportFileType.Pdf, TestContext.Current.CancellationToken);

        report.FileName.ShouldBe("Versuch Ä.pdf");
    }

    [Fact]
    public async Task DownloadReportAsync_NoContentDisposition_FallsBackToExperimentIdAndExtension()
    {
        var (client, _) = CreateClient(_ => ReportResponse([1], PdfMediaType, contentDisposition: null));

        using var report = await client.DownloadReportAsync(ExperimentId, ReportFileType.Pdf, TestContext.Current.CancellationToken);

        report.FileName.ShouldBe($"{ExperimentId}.pdf");
    }

    private static (ExperimentsPublicApiClient Client, StubHttpMessageHandler Handler) CreateClient(
        Func<HttpRequestMessage, HttpResponseMessage> responder)
    {
        var handler = new StubHttpMessageHandler(responder);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://localhost:60479/") };
        return (new ExperimentsPublicApiClient(httpClient), handler);
    }

    private static HttpResponseMessage EventStreamResponse(string body)
    {
        var content = new StringContent(body, Encoding.UTF8);
        content.Headers.ContentType = new MediaTypeHeaderValue("text/event-stream");
        return new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
    }

    private static ContentDispositionHeaderValue Attachment(string fileName) =>
        new("attachment") { FileName = fileName };

    private static HttpResponseMessage ReportResponse(
        byte[] bytes,
        string mediaType,
        ContentDispositionHeaderValue? contentDisposition)
    {
        var content = new ByteArrayContent(bytes);
        content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
        content.Headers.ContentDisposition = contentDisposition;
        return new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
    }

    private static async Task<List<ExperimentStateChangeJson>> ReadAllAsync(
        IAsyncEnumerable<ExperimentStateChangeJson> source)
    {
        var items = new List<ExperimentStateChangeJson>();
        await foreach (var item in source)
        {
            items.Add(item);
        }

        return items;
    }

    private static async Task<byte[]> ReadAllBytesAsync(Stream stream)
    {
        using var memory = new MemoryStream();
        await stream.CopyToAsync(memory);
        return memory.ToArray();
    }
}
