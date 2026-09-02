// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using System.Text.Json.Serialization;

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// Parameters for wait operation
/// </summary>
[JsonDerivedType(typeof(WaitParametersByDuration), "WaitParametersByDuration")]
[JsonDerivedType(typeof(WaitParametersByEndCondition), "WaitParametersByEndCondition")]
[JsonDerivedType(typeof(WaitParametersByExpressionJson), "WaitParametersByExpression")]
public class WaitParametersJson
{
}
