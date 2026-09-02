// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using System.Text.Json.Serialization;

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// Defines the parameters for the <see cref="ControlPhOperationJson"/>
/// </summary>
[JsonDerivedType(typeof(ControlPhParametersRampByRateJson), "ControlPhParametersRampByRate")]
[JsonDerivedType(typeof(ControlPhParametersHoldActualJson), "ControlPhParametersHoldActual")]
[JsonDerivedType(typeof(ControlPhParametersSwitchOffJson), "ControlPhParametersSwitchOff")]
public class ControlPhParametersJson
{
}
