// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using MettlerToledo.Labs.Experiments.Clients.Experiments.PublicApi;

namespace MettlerToledo.Labs.Experiments.iCDataCenterConverter.Exporting;

/// <summary>
/// Writes a downloaded experiment report to storage.
/// </summary>
public interface IReportFileWriter
{
    /// <summary>
    /// Writes the report to storage and returns the full path it was written to.
    /// </summary>
    Task<string> WriteAsync(DownloadedReport report, CancellationToken cancellationToken = default);
}
