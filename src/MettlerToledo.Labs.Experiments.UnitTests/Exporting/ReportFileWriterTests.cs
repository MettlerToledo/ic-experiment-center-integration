// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System.Text;
using MettlerToledo.Labs.Experiments.Clients.Experiments.PublicApi;
using MettlerToledo.Labs.Experiments.iCDataCenterConverter.Exporting;
using Shouldly;
using Xunit;

namespace MettlerToledo.Labs.Experiments.UnitTests.Exporting;

public class ReportFileWriterTests : IDisposable
{
    private readonly string _outputDirectory;
    private readonly ReportFileWriter _writer;

    public ReportFileWriterTests()
    {
        _outputDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_outputDirectory);
        _writer = new ReportFileWriter(_outputDirectory);
    }

    [Fact]
    public async Task WriteAsync_ValidFileName_WritesContentToOutputDirectory()
    {
        var content = Encoding.UTF8.GetBytes("report contents");

        var path = await WriteAsync("My Experiment.pdf", content);

        path.ShouldBe(Path.Combine(_outputDirectory, "My Experiment.pdf"));
        (await File.ReadAllBytesAsync(path, TestContext.Current.CancellationToken)).ShouldBe(content);
    }

    [Theory]
    [InlineData("pH 7: run 2.pdf", "pH 7_ run 2.pdf")]
    [InlineData("Batch 2026/11/25.pdf", "Batch 2026_11_25.pdf")]
    [InlineData("What? <test>|*.pdf", "What_ _test___.pdf")]
    [InlineData("Tab\there.pdf", "Tab_here.pdf")]
    [InlineData("..\\escape.pdf", ".._escape.pdf")]
    [InlineData("../escape.pdf", ".._escape.pdf")]
    [InlineData("..", "_")]
    public async Task WriteAsync_FileNameWithInvalidCharacters_WritesSanitizedFileInOutputDirectory(
        string fileName,
        string expectedFileName)
    {
        var path = await WriteAsync(fileName, [1]);

        path.ShouldBe(Path.Combine(_outputDirectory, expectedFileName));
        File.Exists(path).ShouldBeTrue();
    }

    [Fact]
    public async Task WriteAsync_FileNameAlreadyExists_AppendsNextFreeNumber()
    {
        await WriteAsync("My Experiment.pdf", [1]);
        await WriteAsync("My Experiment.pdf", [2]);

        var path = await WriteAsync("My Experiment.pdf", [3]);

        path.ShouldBe(Path.Combine(_outputDirectory, "My Experiment (2).pdf"));
        (await ReadOutputFileAsync("My Experiment.pdf")).ShouldBe(new byte[] { 1 });
        (await ReadOutputFileAsync("My Experiment (1).pdf")).ShouldBe(new byte[] { 2 });
        (await ReadOutputFileAsync("My Experiment (2).pdf")).ShouldBe(new byte[] { 3 });
    }

    [Fact]
    public async Task WriteAsync_Cancelled_DoesNotLeavePartialFile()
    {
        using var report = new DownloadedReport("My Experiment.pdf", new MemoryStream(new byte[] { 1 }));

        await Should.ThrowAsync<OperationCanceledException>(() =>
            _writer.WriteAsync(report, new CancellationToken(canceled: true)));

        Directory.EnumerateFileSystemEntries(_outputDirectory).ShouldBeEmpty();
    }

    public void Dispose()
    {
        if (Directory.Exists(_outputDirectory))
        {
            Directory.Delete(_outputDirectory, recursive: true);
        }
    }

    private async Task<string> WriteAsync(string fileName, byte[] content)
    {
        using var report = new DownloadedReport(fileName, new MemoryStream(content));
        return await _writer.WriteAsync(report, TestContext.Current.CancellationToken);
    }

    private Task<byte[]> ReadOutputFileAsync(string fileName) =>
        File.ReadAllBytesAsync(Path.Combine(_outputDirectory, fileName), TestContext.Current.CancellationToken);
}
