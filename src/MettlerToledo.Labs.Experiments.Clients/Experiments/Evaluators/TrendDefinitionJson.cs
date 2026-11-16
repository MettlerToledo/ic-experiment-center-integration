// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Evaluators;

/// <summary>
/// Defines a trend produced by an evaluator.
/// </summary>
[JsonDerivedType(typeof(PeakDefinitionJson), "PeakDefinition")]
[JsonDerivedType(typeof(StatisticDefinitionJson), "StatisticDefinition")]
[JsonDerivedType(typeof(TrendDefinitionJson), "TrendDefinition")]
public class TrendDefinitionJson
{
    /// <summary>
    /// The name of the trend.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The units of the trend if specified.
    /// </summary>
    public string? Units { get; set; }

    /// <summary>
    /// A description of the trend.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The parameters of the trend if any.
    /// </summary>
    public IList<EvaluatorParameterJson> Parameters { get; set; } = new List<EvaluatorParameterJson>();
}
