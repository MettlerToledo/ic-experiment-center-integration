// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using AutoChem.Lab.S88Definitions.Parameters;

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// Defines the parameters for ramping the pH to a target value by rate.
/// </summary>
public class ControlPhParametersRampByRateJson : ControlPhParametersActiveJson
{
    /// <summary>
    /// The pH value that the operation tries to control to.
    /// </summary>
    public UnitValueParameterJson Ph { get; set; } = new UnitValueParameterJson();

    /// <summary>
    /// The rate for the pH control.
    /// </summary>
    public UnitValueParameterJson Rate { get; set; } = new UnitValueParameterJson();
}
