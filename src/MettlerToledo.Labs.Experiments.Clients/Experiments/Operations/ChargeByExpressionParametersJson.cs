// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using AutoChem.Lab.S88Definitions.Parameters;

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// The common parameters for a charge operation that uses an expression to determine how much material to charge or how quickly to charge the material portion.
/// </summary>
public abstract class ChargeByExpressionParametersJson : ChargeParametersJson
{
    /// <summary>
    /// When the expression evaluates to true, the charge operation will be suspended until the expression evaluates to false again.
    /// </summary>
    public string? SuspendConditionExpression { get; set; }

    /// <summary>
    /// When the expression evaluates to true, the charge operation will end.
    /// </summary>
    public string? EndConditionExpression { get; set; }

    /// <summary>
    /// The maximum amount that will be charged by expression.
    /// </summary>
    public UnitValueParameterJson MaximumAmount { get; set; } = new UnitValueParameterJson();

    /// <summary>
    /// The response when the charge operation ends by reaching the maximum amount.
    /// </summary>
    public ChargeEndedByMaxAmountResponseJson? ChargeEndedByMaxAmountResponse { get; set; }
}
