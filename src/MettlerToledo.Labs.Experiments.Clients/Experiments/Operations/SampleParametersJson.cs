// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System.Text.Json.Serialization;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

/// <summary>
/// Parameters for the sample operation.
/// </summary>
[JsonDerivedType(typeof(SingleSampleParametersJson), "SingleSampleParameters")]
[JsonDerivedType(typeof(SequenceSampleParametersJson), "SequenceSampleParameters")]
public class SampleParametersJson
{
    /// <summary>
    /// The preparation of of the sample operation.
    /// </summary>
    public string Preparation { get; set; } = string.Empty;
}
