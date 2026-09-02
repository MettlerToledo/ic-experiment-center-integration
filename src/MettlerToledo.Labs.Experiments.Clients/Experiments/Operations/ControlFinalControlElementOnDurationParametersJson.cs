// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using AutoChem.Lab.S88Definitions.Parameters;

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// Turns on the final control element at the specified value for the specified duration.
/// </summary>
public class ControlFinalControlElementOnDurationParametersJson : ControlFinalControlElementParametersJson
{
    /// <summary>
    /// The value for the final control element while it is on.
    /// </summary>
    public UnitValueParameterJson Value { get; set; } = new UnitValueParameterJson();

    /// <summary>
    /// The duration the final control element will be on.
    /// </summary>
    public UnitValueParameterJson Duration { get; set; } = new UnitValueParameterJson();
}
