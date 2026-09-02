// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using System.Text.Json.Serialization;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

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
