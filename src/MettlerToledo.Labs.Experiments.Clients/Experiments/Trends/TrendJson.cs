// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using System;
using System.Collections.Generic;

namespace AutoChem.Lab.S88Definitions.Trends;

/// <summary>
/// A trend.
/// </summary>
public class TrendJson
{
    /// <summary>
    /// The name of the trend.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The unit of the trend.
    /// </summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>
    /// The start time of the trend.
    /// </summary>
    public DateTime StartTimeUtc { get; set; }

    /// <summary>
    /// The values of the trend.
    /// </summary>
    public IList<TrendValueJson> Values { get; set; } = new List<TrendValueJson>();
}
