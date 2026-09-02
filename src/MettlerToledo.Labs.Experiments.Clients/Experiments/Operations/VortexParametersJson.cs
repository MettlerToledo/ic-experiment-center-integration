// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using AutoChem.Lab.S88Definitions.Parameters;

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// The parameters for a vortex operation.
/// </summary>
public class VortexParametersJson : AgitateParametersJson
{
    /// <summary>
    /// The vortex speed.
    /// </summary>
    public UnitValueParameterJson? Speed { get; set; }
}
