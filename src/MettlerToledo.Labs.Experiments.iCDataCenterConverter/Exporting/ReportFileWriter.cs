// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using MettlerToledo.Labs.Experiments.Clients.Experiments.PublicApi;

namespace MettlerToledo.Labs.Experiments.iCDataCenterConverter.Exporting;

/// <summary>
/// Writes downloaded experiment reports to a target directory, using the report's file name.
/// Characters that are not valid in a file name are replaced, and an existing file is never
/// overwritten: a " (1)", " (2)", ... suffix is added instead.
/// </summary>
public sealed class ReportFileWriter : IReportFileWriter
{
    private const char ReplacementCharacter = '_';

    private static readonly char[] InvalidFileNameCharacters = ['<', '>', ':', '"', '/', '\\', '|', '?', '*'];

    private readonly string _outputDirectory;

    /// <summary>
    /// Initializes a new instance of <see cref="ReportFileWriter"/>.
    /// </summary>
    public ReportFileWriter(string outputDirectory)
    {
        _outputDirectory = outputDirectory;
    }

    /// <inheritdoc />
    public async Task<string> WriteAsync(DownloadedReport report, CancellationToken cancellationToken = default)
    {
        var (fileStream, filePath) = CreateNewFile(SanitizeFileName(report.FileName));

        try
        {
            using (fileStream)
            {
                await report.Content.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
            }
        }
        catch
        {
            File.Delete(filePath);
            throw;
        }

        return filePath;
    }

    private static string SanitizeFileName(string fileName)
    {
        var sanitized = new string(fileName
                .Select(character => char.IsControl(character) || InvalidFileNameCharacters.Contains(character)
                    ? ReplacementCharacter
                    : character)
                .ToArray())
            .Trim();

        return sanitized.Trim('.').Length == 0 ? ReplacementCharacter.ToString() : sanitized;
    }

    private (FileStream Stream, string Path) CreateNewFile(string fileName)
    {
        var name = Path.GetFileNameWithoutExtension(fileName);
        var extension = Path.GetExtension(fileName);

        for (var copy = 0; ; copy++)
        {
            var filePath = Path.Combine(_outputDirectory, copy == 0 ? fileName : $"{name} ({copy}){extension}");
            try
            {
                return (new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None), filePath);
            }
            catch (IOException) when (File.Exists(filePath))
            {
                // Name already taken, try the next suffix.
            }
        }
    }
}
