// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using AutoChem.Lab.S88Definitions.Parameters;

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// Parameters for a release operation.
/// </summary>
public class ReleaseParametersDefinedJson : ReleaseParametersJson
{
    /// <summary>
    /// The jacket temperature to maintain.
    /// </summary>
    public UnitValueParameterJson Temperature { get; set; } = new UnitValueParameterJson();

    /// <summary>
    /// The stirring speed.
    /// </summary>
    public UnitValueParameterJson StirringSpeed { get; set; } = new UnitValueParameterJson();
}
