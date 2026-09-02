// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// The parameters for a charge operation that uses an expression to determine the amount to charge.
/// </summary>
public class ChargeByAmountExpressionParametersJson : ChargeByExpressionParametersJson
{
    /// <summary>
    /// An expression that evaluates to the amount to charge.
    /// This expression is evaluated periodically during the charge operation and
    /// the result is used to determine how much material to charge.
    /// </summary>
    public string? AmountExpression { get; set; }
}
