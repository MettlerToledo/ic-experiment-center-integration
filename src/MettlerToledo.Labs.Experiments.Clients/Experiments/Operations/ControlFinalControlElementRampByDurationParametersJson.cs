// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using AutoChem.Lab.S88Definitions.Parameters;

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// Ramp the final control element to the specified value over the specified duration.
/// </summary>
public class ControlFinalControlElementRampByDurationParametersJson : ControlFinalControlElementParametersJson
{
    /// <summary>
    /// The value for the final control element at the end of the operation.
    /// </summary>
    public UnitValueParameterJson EndValue { get; set; } = new UnitValueParameterJson();

    /// <summary>
    /// The duration of the final control ramp.
    /// </summary>
    public UnitValueParameterJson Duration { get; set; } = new UnitValueParameterJson();
}
