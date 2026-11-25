// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.PublicApi;

/// <summary>
/// A client for the iC Experiment Center public experiments API.
/// </summary>
public interface IExperimentsPublicApiClient
{
    /// <summary>
    /// Opens the experiment state change events stream.
    /// </summary>
    IAsyncEnumerable<ExperimentStateChangeJson> StreamStateChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads the experiment report in the requested format.
    /// </summary>
    Task<DownloadedReport> DownloadReportAsync(
        Guid experimentId,
        ReportFileType format,
        CancellationToken cancellationToken = default);
}
