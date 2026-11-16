// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using MettlerToledo.Labs.Experiments.Clients.Experiments.Units;

namespace MettlerToledo.Labs.Experiments.Clients.Definitions.Parameters;

/// <summary>
/// Defines a unit value parameter in a dynamic operation.
/// </summary>
public class UnitValueParameterDefinitionJson : DynamicParameterDefinitionJson
{
    /// <summary>
    /// The unit which the parameter value should be compatible with.
    /// Null indicates that any unit is allowed for the parameter value.
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// The minimum value for the parameter if defined.
    /// The value should be compatible with the unit defined for the parameter.
    /// </summary>
    public UnitValueJson? MinimumValue { get; set; }

    /// <summary>
    /// The maximum value for the parameter if defined.
    /// The value should be compatible with the unit defined for the parameter.
    /// </summary>
    public UnitValueJson? MaximumValue { get; set; }
}
