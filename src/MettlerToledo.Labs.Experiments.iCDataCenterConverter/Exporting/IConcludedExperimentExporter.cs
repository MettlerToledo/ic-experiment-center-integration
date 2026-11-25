// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using MettlerToledo.Labs.Experiments.Clients.Experiments.PublicApi;

namespace MettlerToledo.Labs.Experiments.iCDataCenterConverter.Exporting;

/// <summary>
/// Watches an iC Experiment Center for concluded experiments and exports a report for each one.
/// </summary>
public interface IConcludedExperimentExporter
{
    /// <summary>
    /// Monitors the experiment state change stream and exports a report in the given
    /// file type for every experiment that becomes concluded.
    /// </summary>
    Task Run(ReportFileType fileType, CancellationToken cancellationToken);
}
