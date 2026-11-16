// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Evaluators;

/// <summary>
/// Defines a statistic derived from an XY data set.
/// </summary>
public class StatisticDefinitionJson : TrendDefinitionJson
{
    /// <summary>
    /// The type of statistic that is calculated from the XY data e.g. Mean, Median, Percentile, Standard Deviation, etc.
    /// </summary>
    public string StatisticType { get; set; } = string.Empty;

    /// <summary>
    /// The name of the XY data that is used to calculate the statistic.
    /// </summary>
    public string XYDataName { get; set; } = string.Empty;

    /// <summary>
    /// The name of the Y value that is used to calculate the statistic.
    /// Some sources may have multiple Y values.
    /// </summary>
    public string YValueName { get; set; } = string.Empty;

    /// <summary>
    /// The start of the range of data that is used to calculate the statistic.
    /// </summary>
    public double RangeStart { get; set; }

    /// <summary>
    /// The end of the range of data that is used to calculate the statistic.
    /// </summary>
    public double RangeEnd { get; set; }

    /// <summary>
    /// The type of weighting that is applied to the data when calculating the statistic.
    /// </summary>
    public string WeightingType { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the statistic is calculated as a rate of change.
    /// </summary>
    public bool IsRateOfChange { get; set; }

    /// <summary>
    /// The size of the window used to calculate the rate of change.
    /// </summary>
    public int RateOfChangeWindowSize { get; set; }
}
