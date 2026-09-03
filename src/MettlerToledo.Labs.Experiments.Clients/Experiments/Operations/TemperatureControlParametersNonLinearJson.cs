// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using MettlerToledo.Labs.Experiments.Clients.Experiments.Parameters;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

/// <summary>
/// Parameters for a non-linear temperature ramp
/// </summary>
public class TemperatureControlParametersNonLinearJson : TemperatureControlParametersJson
{
    /// <summary>
    /// The parameter to indicate the type of temperature control.
    /// </summary>
    public TemperatureControlModesJson? TemperatureControlMode { get; set; }

    /// <summary>
    /// The target temperature.
    /// </summary>
    public UnitValueParameterJson Temperature { get; set; } = new();

    /// <summary>
    /// The duration of the ramp to reach the end value.
    /// </summary>
    public UnitValueParameterJson Duration { get; set; } = new();

    /// <summary>
    /// The margin by which the current temperature needs to be within the specified temperature 
    /// for the operation to end.
    /// </summary>
    public UnitValueParameterJson EndValueMargin { get; set; } = new();

    /// <summary>
    /// The exponent for the ramp curve.
    /// </summary>
    public double Exponent { get; set; } = 1.0;
}
