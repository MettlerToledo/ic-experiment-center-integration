// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using AutoChem.Lab.S88Definitions.Parameters;

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// Turns on the final control element with the specified value.
/// </summary>
public class ControlFinalControlElementOnParametersJson : ControlFinalControlElementParametersJson
{
    /// <summary>
    /// The value to control the final control element to.
    /// </summary>
    public UnitValueParameterJson Value { get; set; } = new UnitValueParameterJson();
}
