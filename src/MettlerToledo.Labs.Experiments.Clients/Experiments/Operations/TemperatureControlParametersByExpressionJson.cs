// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using MettlerToledo.Labs.Experiments.Clients.Experiments.Parameters;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

/// <summary>
/// Parameters for controlling the temperature by expression.
/// </summary>
public class TemperatureControlParametersByExpressionJson : TemperatureControlParametersJson
{
    /// <summary>
    /// The parameter to indicate the type of temperature control.
    /// </summary>
    public TemperatureControlModesJson? TemperatureControlMode { get; set; }

    /// <summary>
    /// The expression that will be evaluated periodically during the temperature control operation to determine the target temperature.
    /// </summary>
    public string TemperatureExpression { get; set; } = string.Empty;

    /// <summary>
    /// The minimum temperature that will be used for the temperature control operation.
    /// </summary>
    public UnitValueParameterJson MinimumTemperature { get; set; } = new UnitValueParameterJson();

    /// <summary>
    /// The maximum temperature that will be used for the temperature control operation.
    /// </summary>
    public UnitValueParameterJson MaximumTemperature { get; set; } = new UnitValueParameterJson();

    /// <summary>
    /// When the expression evaluates to true, the temperature control operation will end.
    /// </summary>
    public string? EndConditionExpression { get; set; }
}
