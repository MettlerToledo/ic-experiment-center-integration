// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System.Text.Json.Serialization;

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// The base parameters for a temperature operation.
/// </summary>
[JsonDerivedType(typeof(TemperatureControlParametersRampByDurationJson), "TemperatureControlParametersRampByDuration")]
[JsonDerivedType(typeof(TemperatureControlParametersRampByRateJson), "TemperatureControlParametersRampByRate")]
[JsonDerivedType(typeof(TemperatureControlParametersNonLinearJson), "TemperatureControlParametersNonLinear")]
[JsonDerivedType(typeof(TemperatureControlParametersDeltaJson), "TemperatureControlParametersDelta")]
[JsonDerivedType(typeof(TemperatureControlParametersByExpressionJson), "TemperatureControlParametersByExpression")]
[JsonDerivedType(typeof(TemperatureControlParametersOffJson), "TemperatureControlParametersOff")]
public class TemperatureControlParametersJson
{
}
