// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System;
using System.IO;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.PublicApi;

/// <summary>
/// A downloaded report.
/// </summary>
public sealed class DownloadedReport : IDisposable
{
    private readonly IDisposable? _ownedResource;

    /// <summary>
    /// Initializes a new instance of <see cref="DownloadedReport"/>.
    /// </summary>
    public DownloadedReport(string fileName, Stream content, IDisposable? ownedResource = null)
    {
        FileName = fileName;
        Content = content;
        _ownedResource = ownedResource;
    }

    /// <summary>
    /// The file name reported by the server.
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// The report content stream.
    /// </summary>
    public Stream Content { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        Content.Dispose();
        _ownedResource?.Dispose();
    }
}
