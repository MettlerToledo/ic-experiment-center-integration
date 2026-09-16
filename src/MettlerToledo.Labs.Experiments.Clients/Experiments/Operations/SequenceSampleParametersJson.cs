// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

/// <summary>
/// Parameters for the sample operation using a programmed sequence.
/// </summary>
public class SequenceSampleParametersJson : SampleParametersJson
{
    /// <summary>
    /// The name of the programmed sequence to use for sampling.
    /// </summary>
    public string SequenceName { get; set; } = string.Empty;
}
