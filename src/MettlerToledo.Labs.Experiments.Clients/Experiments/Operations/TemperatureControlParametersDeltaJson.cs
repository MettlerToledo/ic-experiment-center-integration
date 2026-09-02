// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using AutoChem.Lab.S88Definitions.Parameters;

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// Parameters for Jacket and Reactor Delta temperature ramp 
/// </summary>
public class TemperatureControlParametersDeltaJson : TemperatureControlParametersJson
{
    /// <summary>
    /// The jacket temperature at which the operation should complete.
    /// </summary>
    public UnitValueParameterJson EndJacketTemperature { get; set; } = new();

    /// <summary>
    /// The delta between Tj and Tr that is maintained while the operation is running.
    /// </summary>
    public UnitValueParameterJson TjTrDelta { get; set; } = new();
}
