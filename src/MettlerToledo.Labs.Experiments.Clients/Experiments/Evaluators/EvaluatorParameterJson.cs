// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Evaluators;

/// <summary>
/// Defines a parameter for an evaluator.
/// </summary>
public class EvaluatorParameterJson
{
    /// <summary>
    /// The name of the parameter.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The value of the parameter.
    /// An integer, double, string, boolean, or null value can be used.
    /// </summary>
    public object? Value { get; set; }
}
