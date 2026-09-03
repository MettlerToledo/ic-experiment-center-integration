// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

/// <summary>
/// Parameters for controlling the target value by expression.
/// </summary>
public class ControlFinalControlElementByExpressionParametersJson : ControlFinalControlElementParametersJson
{
    /// <summary>
    /// The expression that will be evaluated periodically during the FCE control operation to determine the target value.
    /// </summary>
    public string? ControlExpression { get; set; }

    /// <summary>
    /// When the expression evaluates to true, the FCE control operation will end.
    /// </summary>
    public string? EndConditionExpression { get; set; }
}
