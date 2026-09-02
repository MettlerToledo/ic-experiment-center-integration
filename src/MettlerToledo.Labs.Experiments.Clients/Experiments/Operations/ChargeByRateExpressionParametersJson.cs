// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using AutoChem.Lab.S88Definitions.Parameters;

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// Parameters for a charge operation that uses an expression to determine the rate at which to charge the material portion while
/// the operation is in progress.
/// </summary>
public class ChargeByRateExpressionParametersJson : ChargeByExpressionParametersJson
{
    /// <summary>
    /// An expression that evaluates to the rate at which to charge.
    /// This expression is evaluated periodically during the charge operation and
    /// the result is used to determine how quickly to charge the material portion.
    /// </summary>
    public string? RateExpression { get; set; }

    /// <summary>
    /// The maximum rate that will be used for the charge operation.
    /// </summary>
    public UnitValueParameterJson MaximumRate { get; set; } = new UnitValueParameterJson();
}
