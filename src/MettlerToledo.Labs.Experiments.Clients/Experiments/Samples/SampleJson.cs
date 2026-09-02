// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using System;

namespace AutoChem.Lab.S88Definitions.Samples;

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
