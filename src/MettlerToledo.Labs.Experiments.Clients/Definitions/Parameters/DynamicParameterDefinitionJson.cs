// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System.Text.Json.Serialization;

namespace MettlerToledo.Labs.Experiments.Clients.Definitions.Parameters;

/// <summary>
/// A base class for a dynamic parameter definition.
/// </summary>
[JsonDerivedType(typeof(BooleanParameterDefinitionJson), "BooleanParameterDefinition")]
[JsonDerivedType(typeof(MaterialParameterDefinitionJson), "MaterialParameterDefinition")]
[JsonDerivedType(typeof(StringParameterDefinitionJson), "StringParameterDefinition")]
[JsonDerivedType(typeof(UnitValueParameterDefinitionJson), "UnitValueParameterDefinition")]
public class DynamicParameterDefinitionJson
{
    /// <summary>
    /// The identifier of the parameter in the context of the operation.
    /// It is expected that the user will use the display name when referring to the parameter, and 
    /// internal name will be use for matching the parameters in the system.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The name of the parameter displayed to the user when referring to this parameter.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
}
