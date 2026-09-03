// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using AutoChem.Lab.S88Definitions.Parameters;

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// Parameters for ramping the temperature by duration.
/// </summary>
public class TemperatureControlParametersRampByDurationJson : TemperatureControlParametersJson
{
    /// <summary>
    /// The parameter to indicate the type of temperature control.
    /// </summary>
    public TemperatureControlModesJson? TemperatureControlMode { get; set; }

    /// <summary>
    /// The target temperature.
    /// </summary>
    public UnitValueParameterJson Temperature { get; set; } = new UnitValueParameterJson();

    /// <summary>
    /// The duration of the ramp to reach the end value.
    /// </summary>
    public UnitValueParameterJson Duration { get; set; } = new UnitValueParameterJson();

    /// <summary>
    /// The margin by which the current temperature needs to be within the specified temperature 
    /// for the operation to end.
    /// </summary>
    public UnitValueParameterJson EndValueMargin { get; set; } = new UnitValueParameterJson();

}
