// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

/// <summary>
/// Wait by expression
/// </summary>
public class WaitParametersByExpressionJson : WaitParametersJson
{
    /// <summary>
    /// The user specified boolean expression, when the expression evaluates to true, the wait operation will finish.
    /// </summary>
    public string? Expression { get; set; }
}
