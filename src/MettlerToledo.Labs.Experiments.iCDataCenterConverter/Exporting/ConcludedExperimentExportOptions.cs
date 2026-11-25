// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

namespace MettlerToledo.Labs.Experiments.iCDataCenterConverter.Exporting;

/// <summary>
/// Options for <see cref="ConcludedExperimentExporter"/>.
/// </summary>
public sealed class ConcludedExperimentExportOptions
{
    /// <summary>
    /// The delay before reconnecting after the state change stream is lost. Doubles on each
    /// failure, up to MaxReconnectDelay.
    /// </summary>
    public TimeSpan InitialReconnectDelay { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// The maximum delay between reconnect attempts. A connection that stayed open at least
    /// this long resets the delay to InitialReconnectDelay.
    /// </summary>
    public TimeSpan MaxReconnectDelay { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// The maximum time allowed to download and write a single report.
    /// </summary>
    public TimeSpan ReportExportTimeout { get; set; } = TimeSpan.FromMinutes(5);
}
