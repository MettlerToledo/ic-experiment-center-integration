// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System.Text.Json.Serialization;

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// The parameters for a charge operation.
/// </summary>
[JsonDerivedType(typeof(ChargeByRateParametersJson), "ChargeByRateParameters")]
[JsonDerivedType(typeof(ChargeByDurationParametersJson), "ChargeByDurationParameters")]
[JsonDerivedType(typeof(ChargeByAmountExpressionParametersJson), "ChargeByAmountExpressionParameters")]
[JsonDerivedType(typeof(ChargeByRateExpressionParametersJson), "ChargeByRateExpressionParameters")]
public class ChargeParametersJson
{
}
