// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using MettlerToledo.Labs.Experiments.Clients.Experiments.Materials;

namespace MettlerToledo.Labs.Experiments.Clients.Definitions.Parameters;

/// <summary>
/// Defines a material reference parameter in a dynamic operation.
/// </summary>
public class MaterialParameterDefinitionJson : DynamicParameterDefinitionJson
{
    /// <summary>
    /// The usage of the material portion in the operation.
    /// </summary>
    public MaterialPortionUsageJson MaterialPortionUsage { get; set; } = MaterialPortionUsageJson.Input;
}
