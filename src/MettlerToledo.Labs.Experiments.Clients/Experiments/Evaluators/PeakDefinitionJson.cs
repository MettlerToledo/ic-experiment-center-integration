// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Evaluators;

/// <summary>
/// Defines a peak trend produced by an evaluator.
/// </summary>
public class PeakDefinitionJson : TrendDefinitionJson
{
    /// <summary>
    /// The general measurement method used to generate the peak trend.
    /// </summary>
    public ProfileGroupJson ProfileGroup { get; set; }

    /// <summary>
    /// The specific measurement method used to generate the peak trend.
    /// </summary>
    public string ProfileType { get; set; } = string.Empty;

    /// <summary>
    /// Specifies the wavenumber for calculating peak height or the
    /// starting wavenumber for computing area.
    /// </summary>
    public double PeakStart { get; set; }

    /// <summary>
    /// Specifies the ending wavenumber for calculating peak area or height in a region.
    /// </summary>
    public double PeakStop { get; set; }

    /// <summary>
    /// Specifies the wavenumber for a single point baseline or the
    /// starting wavenumber for a two point baseline.
    /// </summary>
    public double BaselineStart { get; set; }

    /// <summary>
    /// Specifies the ending wavenumber for a two point baseline.  Not
    /// applicable for a single point baseline.
    /// </summary>
    public double BaselineStop { get; set; }
}
