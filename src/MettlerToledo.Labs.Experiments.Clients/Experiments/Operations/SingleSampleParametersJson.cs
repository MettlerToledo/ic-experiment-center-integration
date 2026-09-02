// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using AutoChem.Lab.S88Definitions.Parameters;

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// Parameters for the sample operation taking a single sample.
/// </summary>
public class SingleSampleParametersJson : SampleParametersJson
{
    /// <summary>
    /// The Id of the sample.
    /// </summary>
    public string SampleId { get; set; } = string.Empty;

    /// <summary>
    /// The alternate name of the sample.
    /// </summary>
    public string AlternateName { get; set; } = string.Empty;

    /// <summary>
    /// The quantity to sample
    /// </summary>
    public UnitValueParameterJson Quantity { get; set; } = new UnitValueParameterJson();

    /// <summary>
    /// Represents the factor by which a sample is diluted.
    /// </summary>
    public UnitValueParameterJson DilutionFactor { get; set; } = new UnitValueParameterJson();
}
