// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Samples;

/// <summary>
/// Information about a sample
/// </summary>
public class SampleJson
{
    /// <summary>
    /// The id for the sample
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The time of the sample.
    /// </summary>
    public DateTime TimeUtc { get; set; }
}
