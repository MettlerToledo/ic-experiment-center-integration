// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.


using AutoChem.Lab.S88Definitions.Parameters;

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// Wait by duration
/// </summary>
public class WaitParametersByDuration : WaitParametersJson
{
    /// <summary>
    /// The duration of the wait operation
    /// </summary>
    public UnitValueParameterJson Duration { get; set; } = new UnitValueParameterJson();
}
