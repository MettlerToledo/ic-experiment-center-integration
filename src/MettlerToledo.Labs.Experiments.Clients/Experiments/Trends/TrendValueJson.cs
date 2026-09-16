// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System;
using System.Text.Json.Serialization;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Trends;

/// <summary>
/// A value in a trend.
/// </summary>
public class TrendValueJson
{
    /// <summary>
    /// The relative time in the trend from the start of the trend.
    /// </summary>
    [JsonPropertyName("t")]
    public TimeSpan RelativeTime { get; set; }

    /// <summary>
    /// The value.
    /// </summary>
    [JsonPropertyName("v")]
    public double Value { get; set; }
}
