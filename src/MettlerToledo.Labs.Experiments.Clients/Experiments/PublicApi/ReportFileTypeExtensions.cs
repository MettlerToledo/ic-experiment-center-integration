// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.PublicApi;

/// <summary>
/// Extension helpers for <see cref="ReportFileType"/>.
/// </summary>
public static class ReportFileTypeExtensions
{
    /// <summary>
    /// Gets the file extension for a report file type.
    /// </summary>
    public static string GetFileExtension(this ReportFileType fileType) => fileType switch
    {
        ReportFileType.Docx => ".docx",
        ReportFileType.Pdf => ".pdf",
        _ => throw new ArgumentOutOfRangeException(nameof(fileType), fileType, "Unsupported report file type."),
    };
}
