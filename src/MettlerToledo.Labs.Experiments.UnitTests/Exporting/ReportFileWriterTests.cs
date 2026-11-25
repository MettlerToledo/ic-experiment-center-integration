// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System.IO.Abstractions.TestingHelpers;
using System.Text;
using MettlerToledo.Labs.Experiments.Clients.Experiments.PublicApi;
using MettlerToledo.Labs.Experiments.iCDataCenterConverter.Exporting;
using Shouldly;
using Xunit;

namespace MettlerToledo.Labs.Experiments.UnitTests.Exporting;

public class ReportFileWriterTests
{
    private const string OutputDirectory = @"C:\exports";

    private readonly MockFileSystem _fileSystem = new();
    private readonly ReportFileWriter _writer;

    public ReportFileWriterTests()
    {
        _fileSystem.AddDirectory(OutputDirectory);
        _writer = new ReportFileWriter(_fileSystem, OutputDirectory);
    }

    [Fact]
    public async Task Write_ValidFileName_WritesContentToOutputDirectory()
    {
        var content = Encoding.UTF8.GetBytes("report contents");

        var path = await Write("My Experiment.pdf", content);

        path.ShouldBe(Path.Combine(OutputDirectory, "My Experiment.pdf"));
        _fileSystem.File.ReadAllBytes(path).ShouldBe(content);
    }

    [Theory]
    [InlineData("pH 7: run 2.pdf", "pH 7_ run 2.pdf")]
    [InlineData("Batch 2026/11/25.pdf", "Batch 2026_11_25.pdf")]
    [InlineData("What? <test>|*.pdf", "What_ _test___.pdf")]
    [InlineData("Tab\there.pdf", "Tab_here.pdf")]
    [InlineData("..\\escape.pdf", ".._escape.pdf")]
    [InlineData("../escape.pdf", ".._escape.pdf")]
    [InlineData("..", "_")]
    public async Task Write_FileNameWithInvalidCharacters_WritesSanitizedFileInOutputDirectory(
        string fileName,
        string expectedFileName)
    {
        var path = await Write(fileName, [1]);

        path.ShouldBe(Path.Combine(OutputDirectory, expectedFileName));
        _fileSystem.File.Exists(path).ShouldBeTrue();
    }

    [Fact]
    public async Task Write_FileNameAlreadyExists_AppendsNextFreeNumber()
    {
        await Write("My Experiment.pdf", [1]);
        await Write("My Experiment.pdf", [2]);

        var path = await Write("My Experiment.pdf", [3]);

        path.ShouldBe(Path.Combine(OutputDirectory, "My Experiment (2).pdf"));
        ReadOutputFile("My Experiment.pdf").ShouldBe(new byte[] { 1 });
        ReadOutputFile("My Experiment (1).pdf").ShouldBe(new byte[] { 2 });
        ReadOutputFile("My Experiment (2).pdf").ShouldBe(new byte[] { 3 });
    }

    [Fact]
    public async Task Write_Cancelled_DoesNotLeavePartialFile()
    {
        using var report = new DownloadedReport("My Experiment.pdf", new MemoryStream(new byte[] { 1 }));

        await Should.ThrowAsync<OperationCanceledException>(() =>
            _writer.Write(report, new CancellationToken(canceled: true)));

        _fileSystem.Directory.EnumerateFileSystemEntries(OutputDirectory).ShouldBeEmpty();
    }

    private async Task<string> Write(string fileName, byte[] content)
    {
        using var report = new DownloadedReport(fileName, new MemoryStream(content));
        return await _writer.Write(report, TestContext.Current.CancellationToken);
    }

    private byte[] ReadOutputFile(string fileName) =>
        _fileSystem.File.ReadAllBytes(Path.Combine(OutputDirectory, fileName));
}
